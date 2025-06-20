# WhatsApp Microservice (Baileys)

Este microserviço permite enviar mensagens de texto, áudio, imagem e arquivos (ex: contrato PDF) via WhatsApp, além de receber mensagens via webhook.

## Como rodar

1. Instale as dependências:
   ```bash
   npm install
   ```
2. Inicie o serviço:
   ```bash
   node index.js
   ```
3. No primeiro uso, escaneie o QR code no terminal com o WhatsApp.

## Endpoints

### Enviar texto
```http
POST /send-text
Content-Type: application/json
{
  "to": "5511999999999@s.whatsapp.net",
  "message": "Olá!"
}
```

### Enviar imagem
```http
POST /send-image
Content-Type: multipart/form-data
Campos:
- to: 5511999999999@s.whatsapp.net
- image: arquivo de imagem
- caption: (opcional)
```

### Enviar áudio
```http
POST /send-audio
Content-Type: multipart/form-data
Campos:
- to: 5511999999999@s.whatsapp.net
- audio: arquivo de áudio (mp3, ogg, etc)
```

### Enviar arquivo (ex: contrato)
```http
POST /send-file
Content-Type: multipart/form-data
Campos:
- to: 5511999999999@s.whatsapp.net
- file: arquivo (pdf, doc, etc)
- filename: (opcional)
- caption: (opcional)
```

### Status da conexão
```http
GET /status
```

### Configurar webhook para mensagens recebidas
```http
POST /set-webhook
Content-Type: application/json
{
  "url": "https://seuservidor.com/webhook"
}
```

## Observações
- O número do destinatário deve ser no formato internacional + @s.whatsapp.net (ex: 5511999999999@s.whatsapp.net)
- O serviço armazena a sessão localmente na pasta `auth/`.
- Para rodar em produção, use PM2 ou Docker. 