import { useState, useRef, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageSquare, Send, User, Bot, Gift } from "lucide-react";
import { useToast } from "@/hooks/use-toast";
import { apiRequest } from "@/lib/queryClient";
import { useQuery, useMutation } from "@tanstack/react-query";
import { formatTimeAgo } from "@/lib/utils";

type Message = {
  id: string;
  sender: 'user' | 'bot';
  content: string;
  timestamp: Date;
  confidence?: number;
  hasCoupon?: boolean;
  sentiment?: 'positive' | 'negative' | 'neutral';
};

export function ChatBot() {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState<Message[]>([
    {
      id: "welcome",
      sender: "bot",
      content: "Olá! Tudo bem? Como posso ajudar você hoje com suas dúvidas sobre tratamentos dentários ou estéticos?",
      timestamp: new Date()
    }
  ]);
  const [input, setInput] = useState("");
  const { toast } = useToast();
  const messagesEndRef = useRef<HTMLDivElement>(null);

  // Estado para controlar se está "digitando"
  const [isTyping, setIsTyping] = useState(false);

  // Scroll to bottom of messages
  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: 'smooth' });
    }
  }, [messages]);

  // Detecta sentimentos no texto do usuário
  const detectSentiment = (text: string): 'positive' | 'negative' | 'neutral' => {
    const lowerText = text.toLowerCase();
    
    // Palavras que indicam sentimento negativo ou tristeza
    const negativeWords = ['triste', 'chateado', 'frustrado', 'infeliz', 'preocupado', 'dor', 'sofr', 'caro', 'custa', 'preço', 'magoado', 'ansioso', 'ansiedade', 'medo', 'assustado', 'desapontado', 'decepcionado', 'angustiado', 'não gosto', 'não quero', 'ruim', 'péssimo', 'horrível', 'mal', 'pior'];
    
    // Palavras que indicam sentimento positivo
    const positiveWords = ['feliz', 'animado', 'contente', 'satisfeito', 'ótimo', 'excelente', 'bom', 'bem', 'legal', 'amei', 'gostei', 'top', 'maravilhoso', 'incrível', 'fantástico', 'alegre', 'empolgado', 'tranquilo', 'relaxado', 'confiante', 'adorei', 'sensacional', 'perfeito'];
    
    // Verifica se há palavras negativas no texto
    const hasNegativeWords = negativeWords.some(word => lowerText.includes(word));
    
    // Verifica se há palavras positivas no texto
    const hasPositiveWords = positiveWords.some(word => lowerText.includes(word));
    
    // Determina o sentimento com base nas palavras encontradas
    if (hasNegativeWords && !hasPositiveWords) {
      return 'negative';
    } else if (hasPositiveWords && !hasNegativeWords) {
      return 'positive';
    } else {
      return 'neutral';
    }
  };
  
  // Gera respostas humanizadas com base no sentimento
  const generateHumanizedResponse = (userText: string, sentiment: 'positive' | 'negative' | 'neutral'): { content: string, hasCoupon: boolean } => {
    // Trechos de saudações e perguntas de bem-estar que podem estar no texto do usuário
    const greetings = ['oi', 'olá', 'ola', 'e aí', 'eai', 'bom dia', 'boa tarde', 'boa noite', 'tudo bem', 'como vai', 'como está'];
    const lowerText = userText.toLowerCase();
    const isGreeting = greetings.some(greeting => lowerText.includes(greeting));
    
    // Se for uma saudação, responde de forma adequada ao sentimento
    if (isGreeting) {
      if (sentiment === 'positive') {
        return {
          content: "Olá! Que bom que está feliz hoje! Como posso ajudar com seus tratamentos ou agendar uma consulta?",
          hasCoupon: false
        };
      } else if (sentiment === 'negative') {
        return {
          content: "Olá! Percebo que você não está se sentindo muito bem hoje. Gostaria de oferecer um cupom de 15% de desconto em nossos serviços para alegrar seu dia! Use o código SORRIA15 em sua próxima visita. Posso ajudar em algo específico?",
          hasCoupon: true
        };
      } else {
        return {
          content: "Olá! Tudo bem por aqui! Como posso ajudar você hoje?",
          hasCoupon: false
        };
      }
    }
    
    // Se o usuário estiver com sentimento negativo, oferecer desconto
    if (sentiment === 'negative') {
      return {
        content: "Sinto muito que você esteja passando por isso. Para melhorar seu dia, gostaria de oferecer um cupom de desconto de 15% em seu próximo tratamento. Use o código SORRIA15. Como posso ajudar com sua situação específica?",
        hasCoupon: true
      };
    }
    
    // Respostas padrão para outros casos
    return {
      content: "Entendi. Como posso ajudar você com nossos serviços dentários ou estéticos hoje?",
      hasCoupon: false
    };
  };

  const handleSendMessage = () => {
    if (!input.trim()) return;
    
    // Detecta o sentimento do texto do usuário
    const sentiment = detectSentiment(input);
    
    const userMessage: Message = {
      id: Date.now().toString(),
      sender: 'user',
      content: input,
      timestamp: new Date(),
      sentiment
    };
    
    setMessages(prev => [...prev, userMessage]);
    
    // Gera uma resposta humanizada com base no sentimento
    const humanizedResponse = generateHumanizedResponse(input, sentiment);
    
    // Indica que o bot está digitando
    setIsTyping(true);
    
    // Simula o tempo de resposta do assistente (entre 500ms e 1500ms)
    setTimeout(() => {
      const botMessage: Message = {
        id: Date.now().toString(),
        sender: 'bot',
        content: humanizedResponse.content,
        timestamp: new Date(),
        hasCoupon: humanizedResponse.hasCoupon,
        sentiment: 'neutral' // o bot sempre tem sentimento neutro
      };
      
      setMessages(prev => [...prev, botMessage]);
      setIsTyping(false);
    }, Math.random() * 1000 + 500);
    
    setInput("");
  };

  return (
    <>
      <Button
        onClick={() => setIsOpen(!isOpen)}
        size="icon"
        className="fixed bottom-6 right-6 h-12 w-12 rounded-full shadow-lg"
      >
        <MessageSquare />
      </Button>

      {isOpen && (
        <Card className="fixed bottom-20 right-6 w-80 sm:w-96 shadow-lg z-50">
          <CardHeader className="pb-2">
            <CardTitle className="text-lg flex items-center gap-2">
              <Bot className="h-5 w-5 text-primary" />
              <span>Assistente DentalSpa</span>
            </CardTitle>
          </CardHeader>
          <ScrollArea className="h-[300px] px-4" type="always">
            <CardContent className="space-y-4 pt-0">
              {messages.map((message) => (
                <div
                  key={message.id}
                  className={`flex ${message.sender === 'user' ? 'justify-end' : 'justify-start'}`}
                >
                  <div
                    className={`max-w-[80%] rounded-lg px-3 py-2 ${
                      message.sender === 'user'
                        ? message.sentiment === 'negative'
                          ? 'bg-red-500 text-primary-foreground'
                          : message.sentiment === 'positive'
                            ? 'bg-green-500 text-primary-foreground'
                            : 'bg-primary text-primary-foreground'
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
                      {message.confidence && (
                        <Badge variant="outline" className="ml-1 text-xs">
                          {message.confidence}% correspondência
                        </Badge>
                      )}
                    </div>
                    <p className="text-sm">{message.content}</p>
                    {message.hasCoupon && (
                      <div className="mt-2 p-2 bg-primary/10 rounded-md border border-primary/30">
                        <div className="flex items-center gap-2 mb-1">
                          <Gift className="h-3 w-3 text-primary" />
                          <span className="text-xs font-medium text-primary">Cupom de Desconto</span>
                        </div>
                        <p className="text-sm font-bold text-center">SORRIA15</p>
                        <p className="text-xs text-center">15% de desconto no próximo atendimento</p>
                      </div>
                    )}
                    <p className="text-xs opacity-70 mt-1 text-right">
                      {formatTimeAgo(message.timestamp)}
                    </p>
                  </div>
                </div>
              ))}
              <div ref={messagesEndRef} />
            </CardContent>
          </ScrollArea>
          <CardFooter className="border-t p-2">
            <div className="flex w-full items-center space-x-2">
              <Input
                placeholder="Digite sua mensagem..."
                value={input}
                onChange={(e) => setInput(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSendMessage()}
                disabled={sendMessage.isPending}
              />
              <Button 
                size="icon" 
                onClick={handleSendMessage}
                disabled={sendMessage.isPending}
              >
                <Send className="h-4 w-4" />
              </Button>
            </div>
          </CardFooter>
        </Card>
      )}
    </>
  );
}

export default ChatBot;
