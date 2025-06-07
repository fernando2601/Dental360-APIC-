#!/usr/bin/env node

const { spawn } = require('child_process');
const path = require('path');

console.log('🦷 Starting DentalSpa Development Server...');

// Start the Express server that serves the Angular app
const server = spawn('node', ['server.js'], {
  stdio: 'inherit',
  cwd: __dirname
});

server.on('error', (err) => {
  console.error('❌ Failed to start server:', err);
  process.exit(1);
});

server.on('close', (code) => {
  console.log(`Server process exited with code ${code}`);
  process.exit(code);
});

// Handle shutdown gracefully
process.on('SIGINT', () => {
  console.log('\n🔄 Shutting down development server...');
  server.kill('SIGINT');
});

process.on('SIGTERM', () => {
  console.log('\n🔄 Shutting down development server...');
  server.kill('SIGTERM');
});