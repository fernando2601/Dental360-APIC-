import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-whatsapp',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fab fa-whatsapp me-2"></i>WhatsApp Business</h2>
        <div class="d-flex gap-2">
          <button class="btn btn-success">
            <i class="fab fa-whatsapp me-2"></i>Conectar WhatsApp
          </button>
          <button class="btn btn-primary">
            <i class="fas fa-paper-plane me-2"></i>Nova Mensagem
          </button>
        </div>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-comments text-success fa-2x mb-2"></i>
              <h6>Mensagens Hoje</h6>
              <h4 class="text-success">{{stats.messagestoday}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-clock text-warning fa-2x mb-2"></i>
              <h6>Pendentes</h6>
              <h4 class="text-warning">{{stats.pending}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-robot text-info fa-2x mb-2"></i>
              <h6>Respostas Automáticas</h6>
              <h4 class="text-info">{{stats.automated}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-percentage text-primary fa-2x mb-2"></i>
              <h6>Taxa de Resposta</h6>
              <h4 class="text-primary">{{stats.responseRate}}%</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="row">
        <div class="col-md-4">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white d-flex justify-content-between align-items-center">
              <h6 class="mb-0">Conversas Recentes</h6>
              <div class="input-group" style="width: 200px;">
                <span class="input-group-text">
                  <i class="fas fa-search"></i>
                </span>
                <input type="text" class="form-control" placeholder="Buscar..." [(ngModel)]="searchTerm">
              </div>
            </div>
            <div class="card-body p-0" style="max-height: 500px; overflow-y: auto;">
              <div *ngFor="let conversation of conversations" 
                   class="conversation-item p-3 border-bottom"
                   [class.active]="selectedConversation?.id === conversation.id"
                   (click)="selectConversation(conversation)">
                <div class="d-flex align-items-center">
                  <div class="avatar-sm bg-success rounded-circle d-flex align-items-center justify-content-center me-3">
                    <i class="fab fa-whatsapp text-white"></i>
                  </div>
                  <div class="flex-grow-1">
                    <div class="d-flex justify-content-between align-items-start">
                      <h6 class="mb-1">{{conversation.contactName}}</h6>
                      <small class="text-muted">{{formatTime(conversation.lastMessageTime)}}</small>
                    </div>
                    <p class="mb-1 small text-muted">{{conversation.lastMessage}}</p>
                    <div class="d-flex justify-content-between align-items-center">
                      <small class="text-muted">{{conversation.phone}}</small>
                      <span *ngIf="conversation.unreadCount > 0" class="badge bg-success rounded-pill">
                        {{conversation.unreadCount}}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-8">
          <div class="card border-0 shadow-sm" style="height: 600px; display: flex; flex-direction: column;">
            <div class="card-header bg-white" *ngIf="selectedConversation">
              <div class="d-flex justify-content-between align-items-center">
                <div class="d-flex align-items-center">
                  <div class="avatar-sm bg-success rounded-circle d-flex align-items-center justify-content-center me-3">
                    <i class="fab fa-whatsapp text-white"></i>
                  </div>
                  <div>
                    <h6 class="mb-0">{{selectedConversation.contactName}}</h6>
                    <small class="text-muted">{{selectedConversation.phone}}</small>
                  </div>
                </div>
                <div class="btn-group" role="group">
                  <button class="btn btn-sm btn-outline-primary" title="Informações do Contato">
                    <i class="fas fa-info-circle"></i>
                  </button>
                  <button class="btn btn-sm btn-outline-success" title="Agendar Consulta">
                    <i class="fas fa-calendar-plus"></i>
                  </button>
                  <button class="btn btn-sm btn-outline-secondary" title="Histórico">
                    <i class="fas fa-history"></i>
                  </button>
                </div>
              </div>
            </div>

            <div class="card-body p-3" style="flex: 1; overflow-y: auto;" *ngIf="selectedConversation">
              <div class="messages-container">
                <div *ngFor="let message of selectedMessages" 
                     class="message-item mb-3"
                     [class.sent]="message.type === 'sent'"
                     [class.received]="message.type === 'received'">
                  <div class="message-bubble p-2 rounded">
                    <p class="mb-1">{{message.content}}</p>
                    <small class="text-muted">{{formatTime(message.timestamp)}}</small>
                  </div>
                </div>
              </div>
            </div>

            <div class="card-footer bg-white" *ngIf="selectedConversation">
              <div class="d-flex gap-2">
                <button class="btn btn-outline-secondary">
                  <i class="fas fa-paperclip"></i>
                </button>
                <button class="btn btn-outline-secondary">
                  <i class="fas fa-smile"></i>
                </button>
                <input type="text" class="form-control" placeholder="Digite sua mensagem..." [(ngModel)]="newMessage" (keyup.enter)="sendMessage()">
                <button class="btn btn-success" (click)="sendMessage()">
                  <i class="fas fa-paper-plane"></i>
                </button>
              </div>
            </div>

            <div class="card-body d-flex align-items-center justify-content-center" *ngIf="!selectedConversation">
              <div class="text-center text-muted">
                <i class="fab fa-whatsapp fa-3x mb-3"></i>
                <h5>Selecione uma conversa</h5>
                <p>Escolha uma conversa para visualizar as mensagens</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <style>
      .conversation-item {
        cursor: pointer;
        transition: background-color 0.2s;
      }
      .conversation-item:hover {
        background-color: #f8f9fa;
      }
      .conversation-item.active {
        background-color: #e3f2fd;
        border-left: 4px solid #2196f3;
      }
      .message-item.sent {
        text-align: right;
      }
      .message-item.sent .message-bubble {
        background-color: #dcf8c6;
        display: inline-block;
        max-width: 70%;
      }
      .message-item.received .message-bubble {
        background-color: #ffffff;
        border: 1px solid #e0e0e0;
        display: inline-block;
        max-width: 70%;
      }
      .messages-container {
        display: flex;
        flex-direction: column;
      }
    </style>
  `
})
export class WhatsAppComponent implements OnInit {
  stats = { 
    messagestoday: 0, 
    pending: 0, 
    automated: 0, 
    responseRate: 0 
  };
  conversations: any[] = [];
  selectedConversation: any = null;
  selectedMessages: any[] = [];
  searchTerm = '';
  newMessage = '';

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadWhatsAppData();
  }

  loadWhatsAppData() {
    this.loadStats();
    this.loadConversations();
  }

  loadStats() {
    this.apiService.get('/api/whatsapp/stats').subscribe({
      next: (data: any) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  loadConversations() {
    this.apiService.get('/api/whatsapp/conversations').subscribe({
      next: (data: any) => {
        this.conversations = data;
      },
      error: (error) => {
        console.error('Erro ao carregar conversas:', error);
      }
    });
  }

  selectConversation(conversation: any) {
    this.selectedConversation = conversation;
    this.loadMessages(conversation.id);
  }

  loadMessages(conversationId: string) {
    this.apiService.get(`/api/whatsapp/messages/${conversationId}`).subscribe({
      next: (data: any) => {
        this.selectedMessages = data;
      },
      error: (error) => {
        console.error('Erro ao carregar mensagens:', error);
      }
    });
  }

  sendMessage() {
    if (this.newMessage.trim() && this.selectedConversation) {
      const message = {
        conversationId: this.selectedConversation.id,
        content: this.newMessage,
        type: 'sent'
      };

      this.apiService.post('/api/whatsapp/send', message).subscribe({
        next: () => {
          this.selectedMessages.push({
            ...message,
            timestamp: new Date().toISOString()
          });
          this.newMessage = '';
        },
        error: (error) => {
          console.error('Erro ao enviar mensagem:', error);
        }
      });
    }
  }

  formatTime(timestamp: string): string {
    return new Date(timestamp).toLocaleTimeString('pt-BR', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}