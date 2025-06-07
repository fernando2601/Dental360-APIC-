const http = require('http');
const fs = require('fs');
const path = require('path');

const port = 4200;

const server = http.createServer((req, res) => {
  let filePath;
  
  if (req.url === '/') {
    filePath = path.join(__dirname, 'demo.html');
  } else if (req.url.startsWith('/src/')) {
    filePath = path.join(__dirname, req.url);
  } else {
    filePath = path.join(__dirname, 'demo.html');
  }

  const extname = String(path.extname(filePath)).toLowerCase();
  const mimeTypes = {
    '.html': 'text/html',
    '.js': 'text/javascript',
    '.css': 'text/css',
    '.json': 'application/json',
    '.png': 'image/png',
    '.jpg': 'image/jpg',
    '.gif': 'image/gif',
    '.svg': 'image/svg+xml',
    '.wav': 'audio/wav',
    '.mp4': 'video/mp4',
    '.woff': 'application/font-woff',
    '.ttf': 'application/font-ttf',
    '.eot': 'application/vnd.ms-fontobject',
    '.otf': 'application/font-otf',
    '.wasm': 'application/wasm'
  };

  const contentType = mimeTypes[extname] || 'application/octet-stream';

  fs.readFile(filePath, (error, content) => {
    if (error) {
      if (error.code == 'ENOENT') {
        res.writeHead(404, { 'Content-Type': 'text/html' });
        res.end('<h1>404 - Page Not Found</h1>', 'utf-8');
      } else {
        res.writeHead(500);
        res.end('Sorry, check with the site admin for error: ' + error.code + ' ..\n');
      }
    } else {
      res.writeHead(200, { 'Content-Type': contentType });
      res.end(content, 'utf-8');
    }
  });
});

server.listen(port, '0.0.0.0', () => {
  console.log(`DentalSpa server running at http://0.0.0.0:${port}/`);
  console.log('Frontend Angular restaurado com sucesso!');
});