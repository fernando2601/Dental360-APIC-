const { default: makeWASocket, useMultiFileAuthState, fetchLatestBaileysVersion, DisconnectReason } = require('@whiskeysockets/baileys');
const express = require('express');
const multer = require('multer');
const cors = require('cors');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 3001;

app.use(cors());
app.use(express.json());

// Configuração de upload
const upload = multer({ dest: 'uploads/' });

// Webhook para mensagens recebidas
let webhookUrl = null;
app.post('/set-webhook', (req, res) => {
    webhookUrl = req.body.url;
    res.json({ success: true, webhookUrl });
});

// Baileys Auth
const SESSION_FOLDER = './auth';
if (!fs.existsSync(SESSION_FOLDER)) fs.mkdirSync(SESSION_FOLDER);

let sock;

async function startSock() {
    const { state, saveCreds } = await useMultiFileAuthState(SESSION_FOLDER);
    const { version } = await fetchLatestBaileysVersion();
    sock = makeWASocket({
        version,
        auth: state,
        printQRInTerminal: true,
        syncFullHistory: false,
    });
    sock.ev.on('creds.update', saveCreds);
    sock.ev.on('messages.upsert', async ({ messages }) => {
        if (!messages || !messages[0]) return;
        const msg = messages[0];
        if (webhookUrl) {
            try {
                await fetch(webhookUrl, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(msg)
                });
            } catch (e) { console.error('Erro ao enviar webhook:', e); }
        }
    });
    sock.ev.on('connection.update', (update) => {
        if (update.connection === 'close' && update.lastDisconnect?.error?.output?.statusCode !== DisconnectReason.loggedOut) {
            startSock();
        }
    });
}

startSock();

// Envio de texto
app.post('/send-text', async (req, res) => {
    const { to, message } = req.body;
    if (!to || !message) return res.status(400).json({ error: 'to e message são obrigatórios' });
    try {
        await sock.sendMessage(to, { text: message });
        res.json({ success: true });
    } catch (e) {
        res.status(500).json({ error: e.message });
    }
});

// Envio de imagem
app.post('/send-image', upload.single('image'), async (req, res) => {
    const { to, caption } = req.body;
    if (!to || !req.file) return res.status(400).json({ error: 'to e image são obrigatórios' });
    try {
        const buffer = fs.readFileSync(req.file.path);
        await sock.sendMessage(to, { image: buffer, caption });
        fs.unlinkSync(req.file.path);
        res.json({ success: true });
    } catch (e) {
        res.status(500).json({ error: e.message });
    }
});

// Envio de áudio
app.post('/send-audio', upload.single('audio'), async (req, res) => {
    const { to } = req.body;
    if (!to || !req.file) return res.status(400).json({ error: 'to e audio são obrigatórios' });
    try {
        const buffer = fs.readFileSync(req.file.path);
        await sock.sendMessage(to, { audio: buffer, mimetype: 'audio/mp4' });
        fs.unlinkSync(req.file.path);
        res.json({ success: true });
    } catch (e) {
        res.status(500).json({ error: e.message });
    }
});

// Envio de arquivo (ex: contrato)
app.post('/send-file', upload.single('file'), async (req, res) => {
    const { to, filename, caption } = req.body;
    if (!to || !req.file) return res.status(400).json({ error: 'to e file são obrigatórios' });
    try {
        const buffer = fs.readFileSync(req.file.path);
        await sock.sendMessage(to, {
            document: buffer,
            fileName: filename || req.file.originalname,
            mimetype: req.file.mimetype,
            caption: caption || ''
        });
        fs.unlinkSync(req.file.path);
        res.json({ success: true });
    } catch (e) {
        res.status(500).json({ error: e.message });
    }
});

// Status
app.get('/status', (req, res) => {
    res.json({ connected: !!sock?.user, user: sock?.user });
});

app.listen(PORT, () => {
    console.log('WhatsApp service rodando na porta', PORT);
}); 