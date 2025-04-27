import { ChatBotLite } from "@/components/chat-bot-lite";

export default function Chat() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Chat da Clínica</h1>
        <p className="text-muted-foreground">Converse com nosso assistente virtual para dúvidas e agendamentos.</p>
      </div>
      
      <ChatBotLite />
    </div>
  );
}