import { useState, useRef, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageSquare, Send, User, Bot } from "lucide-react";

type Message = {
  id: string;
  sender: 'user' | 'bot';
  content: string;
  timestamp: Date;
};

export function ChatBotLite() {
  const [input, setInput] = useState("");
  const [messages, setMessages] = useState<Message[]>([]);
  const [isTyping, setIsTyping] = useState(false);
  
  const messagesEndRef = useRef<HTMLDivElement>(null);

  // Efeito para inicializar o chatbot com uma mensagem de boas-vindas
  useEffect(() => {
    if (messages.length === 0) {
      setMessages([
        {
          id: "welcome-message",
          sender: "bot",
          content: "Olá! 😊 Bem-vindo à nossa clínica de harmonização e odontologia. Como você está hoje?",
          timestamp: new Date(),
        }
      ]);
    }
  }, [messages]);

  // Rola para a mensagem mais recente
  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [messages]);

  // Respostas para palavras-chave específicas
  const getKeywordResponse = (text: string): string | null => {
    const lowerText = text.toLowerCase();
    
    if (lowerText.includes("dente")) {
      return "Seu sorriso merece o melhor cuidado! 😁 Somos especialistas em transformar sorrisos. Podemos agendar uma avaliação sem compromisso?";
    }
    
    if (lowerText.includes("sorriso")) {
      return "Um sorriso bonito abre portas! 😁 E nossos especialistas são mestres em criar sorrisos perfeitos. Posso te mostrar alguns antes/depois?";
    }
    
    if (lowerText.includes("harmonização") || lowerText.includes("botox")) {
      return "Nossa harmonização facial é referência! 💎 Resultados naturais que realçam sua beleza sem exageros. Quer conhecer nosso portfólio?";
    }
    
    if (lowerText.includes("meu dente tá podre") || lowerText.includes("dente podre")) {
      return "Fica tranquilo(a)! Nós somos especialistas em salvar sorrisos! ❤️ Dá pra restaurar ou até reconstruir o dente, dependendo do caso. Vamos agendar uma avaliação sem compromisso?";
    }
    
    return null;
  };

  // Envia a mensagem e simula uma resposta
  const handleSendMessage = () => {
    if (!input.trim()) return;
    
    // Adiciona a mensagem do usuário
    const userMessage: Message = {
      id: Date.now().toString(),
      sender: 'user',
      content: input,
      timestamp: new Date()
    };
    
    setMessages(prev => [...prev, userMessage]);
    setInput("");
    
    // Simula o chatbot digitando
    setIsTyping(true);
    
    // Gera a resposta após um pequeno delay
    setTimeout(() => {
      // Verifica palavras-chave para respostas específicas
      const keywordResponse = getKeywordResponse(input);
      
      // Caso não encontre correspondência, usa resposta padrão
      const responseContent = keywordResponse || "Obrigado pelo contato! 😊 Estamos prontos para te ajudar a conquistar o sorriso dos seus sonhos. Que tal agendar uma avaliação sem compromisso?";
      
      const botResponse: Message = {
        id: Date.now().toString(),
        sender: 'bot',
        content: responseContent,
        timestamp: new Date()
      };
      
      setMessages(prev => [...prev, botResponse]);
      setIsTyping(false);
    }, 1500);
  };

  // Formatação da data/hora
  const formatTimeAgo = (date: Date): string => {
    const now = new Date();
    const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);
    
    if (diffInSeconds < 60) return `Agora mesmo`;
    if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)} min atrás`;
    if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)} h atrás`;
    return date.toLocaleDateString();
  };

  return (
    <Card className="w-full max-w-4xl mx-auto">
      <CardHeader className="border-b">
        <CardTitle className="flex items-center gap-2">
          <MessageSquare className="h-5 w-5" />
          Chat com a Clínica
        </CardTitle>
      </CardHeader>
      <CardContent className="p-0">
        <ScrollArea className="h-[500px] p-4">
          {messages.map((message) => (
            <div
              key={message.id}
              className={`flex ${
                message.sender === 'user' ? 'justify-end' : 'justify-start'
              } mb-4`}
            >
              <div
                className={`max-w-[85%] rounded-lg px-3 py-2 ${
                  message.sender === 'user'
                    ? 'bg-primary text-primary-foreground'
                    : 'bg-muted'
                }`}
              >
                <div className="flex items-center gap-2 mb-1">
                  {message.sender === 'user' ? (
                    <User className="h-3 w-3" />
                  ) : (
                    <Bot className="h-3 w-3" />
                  )}
                  <span className="text-xs font-medium">
                    {message.sender === 'user' ? 'Você' : 'Assistente'}
                  </span>
                </div>
                
                <div className="whitespace-pre-wrap">{message.content}</div>
                
                <p className="text-xs opacity-70 mt-1 text-right">
                  {formatTimeAgo(message.timestamp)}
                </p>
              </div>
            </div>
          ))}
          
          {isTyping && (
            <div className="flex justify-start">
              <div className="max-w-[80%] rounded-lg px-3 py-2 bg-muted">
                <div className="flex items-center gap-2 mb-1">
                  <Bot className="h-3 w-3" />
                  <span className="text-xs font-medium">Assistente</span>
                </div>
                <div className="flex items-center gap-1">
                  <div className="w-2 h-2 rounded-full bg-primary animate-bounce" style={{ animationDelay: '0ms' }}></div>
                  <div className="w-2 h-2 rounded-full bg-primary animate-bounce" style={{ animationDelay: '200ms' }}></div>
                  <div className="w-2 h-2 rounded-full bg-primary animate-bounce" style={{ animationDelay: '400ms' }}></div>
                </div>
              </div>
            </div>
          )}
          
          <div ref={messagesEndRef} />
        </ScrollArea>
      </CardContent>
      <CardFooter className="border-t p-2">
        <div className="flex w-full items-center gap-2">
          <Input
            placeholder="Digite sua mensagem..."
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && !isTyping && handleSendMessage()}
            className="flex-1"
          />
          <Button 
            onClick={handleSendMessage} 
            disabled={isTyping}
            size="icon"
            className="shrink-0"
          >
            <Send className="h-4 w-4" />
          </Button>
        </div>
      </CardFooter>
    </Card>
  );
}