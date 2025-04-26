import { useState, useRef, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageSquare, Send, User, Bot, Gift, MoreHorizontal, Calendar, Info, ThumbsUp } from "lucide-react";
import { useToast } from "@/hooks/use-toast";
import { formatTimeAgo } from "@/lib/utils";

type Message = {
  id: string;
  sender: 'user' | 'bot';
  content: string;
  timestamp: Date;
  confidence?: number;
  hasCoupon?: boolean;
  sentiment?: 'positive' | 'negative' | 'neutral';
  showServicesInfo?: boolean;
  showScheduleInfo?: boolean;
  showClinicInfo?: boolean;
};

export function ChatBot() {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState<Message[]>([
    {
      id: "welcome",
      sender: "bot",
      content: "Olá! Seja MUITO bem-vindo(a) à nossa clínica ✨\nEu sou o assistente virtual mais animado do Brasil! 😁\nComo você está hoje?",
      timestamp: new Date()
    }
  ]);
  const [input, setInput] = useState("");
  const { toast } = useToast();
  const messagesEndRef = useRef<HTMLDivElement>(null);

  // Estado para controlar se está "digitando"
  const [isTyping, setIsTyping] = useState(false);
  
  // Serviços e preços
  const services = {
    dental: [
      { name: "Limpeza", price: 120 },
      { name: "Clareamento", price: 400 },
      { name: "Tratamento de cárie", price: 250 },
      { name: "Aparelho ortodôntico (manutenção)", price: 180 },
      { name: "Implante dentário", price: 1800 }
    ],
    harmonization: [
      { name: "Botox", price: 500 },
      { name: "Preenchimento labial", price: 650 },
      { name: "Bichectomia", price: 1200 },
      { name: "Lifting facial com fios de PDO", price: 2000 },
      { name: "Bioestimulador de colágeno", price: 800 }
    ]
  };

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
    const negativeWords = ['triste', 'chateado', 'frustrado', 'infeliz', 'preocupado', 'dor', 'sofr', 'caro', 'custa', 'preço', 'magoado', 
    'ansioso', 'ansiedade', 'medo', 'assustado', 'desapontado', 'decepcionado', 'angustiado', 'não gosto', 'não quero', 
    'ruim', 'péssimo', 'horrível', 'mal', 'pior', 'não estou bem', 'doente', 'cansado', 'estressado'];
    
    // Palavras que indicam sentimento positivo
    const positiveWords = ['feliz', 'animado', 'contente', 'satisfeito', 'ótimo', 'excelente', 'bom', 'bem', 'legal', 
    'amei', 'gostei', 'top', 'maravilhoso', 'incrível', 'fantástico', 'alegre', 'empolgado', 'tranquilo', 
    'relaxado', 'confiante', 'adorei', 'sensacional', 'perfeito', 'estou bem', 'tudo ótimo'];
    
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
  
  // Gera respostas humanizadas com base no script e no sentimento
  const generateHumanizedResponse = (userText: string, sentiment: 'positive' | 'negative' | 'neutral'): Message => {
    const lowerText = userText.toLowerCase();
    
    // Verifica se é uma resposta sobre como o usuário está se sentindo (logo após a primeira mensagem)
    if (messages.length === 1 || 
        (messages[messages.length-2]?.content.includes("Como você está hoje?") && 
         messages[messages.length-1].sender === 'user')) {
      
      if (sentiment === 'positive') {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Que alegria! 😍 Vamos deixar seu sorriso ainda mais incrível!\nPosso te ajudar a encontrar o serviço ideal?",
          timestamp: new Date(),
          sentiment: 'neutral',
          hasCoupon: false
        };
      } else if (sentiment === 'negative') {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Poxa, sinto muito por isso. 😔\nPara melhorar seu dia, aqui vai um presente especial 🎁:\n**CUPOM DE DESCONTO DE 10%** para qualquer procedimento hoje!\n\nQuer que eu te ajude a agendar seu horário? 💬",
          timestamp: new Date(),
          sentiment: 'neutral',
          hasCoupon: true
        };
      }
    }
    
    // Verifica se está perguntando sobre preços ou serviços
    if (lowerText.includes("preço") || lowerText.includes("valor") || lowerText.includes("custo") || 
        lowerText.includes("quanto custa") || lowerText.includes("serviço") || lowerText.includes("procedimento") ||
        lowerText.includes("tratamento")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Esses são alguns dos nossos procedimentos mais procurados! 💖\nPosso te passar mais detalhes sobre qualquer um deles! 👩‍⚕️👨‍⚕️",
        timestamp: new Date(),
        sentiment: 'neutral',
        showServicesInfo: true
      };
    }
    
    // Verifica se está perguntando sobre agendamento
    if (lowerText.includes("agen") || lowerText.includes("marcar") || lowerText.includes("consulta") || 
        lowerText.includes("horário") || lowerText.includes("disponib") || lowerText.includes("atendimento")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Gostaria de agendar uma avaliação gratuita? 📅\nTemos horários incríveis essa semana!\nPosso ver qual o melhor para você?",
        timestamp: new Date(),
        sentiment: 'neutral',
        showScheduleInfo: true
      };
    }
    
    // Verifica se está perguntando sobre a clínica
    if (lowerText.includes("clínica") || lowerText.includes("lugar") || lowerText.includes("estabelecimento") || 
        lowerText.includes("diferencial") || lowerText.includes("vantagem") || lowerText.includes("por que escolher") ||
        lowerText.includes("profissionais") || lowerText.includes("equipe") || lowerText.includes("médicos")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Por que escolher a nossa clínica? 😍\n\n✨ Profissionais premiados e apaixonados pelo que fazem\n✨ Atendimento acolhedor e humanizado\n✨ Equipamentos modernos para seu conforto e segurança\n✨ Resultados naturais e personalizados para você!\n\nAqui você não é só mais um paciente, você é parte da nossa família 💖",
        timestamp: new Date(),
        sentiment: 'neutral',
        showClinicInfo: true
      };
    }
    
    // Se o usuário demonstrar dúvida ou confusão
    if (lowerText.includes("não entendi") || lowerText.includes("confuso") || lowerText.includes("?") || 
        lowerText.includes("como assim") || lowerText.includes("não sei") || lowerText.includes("explica")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sem problema! Estou aqui para te ajudar com calma! 🫶\nSe eu não expliquei direito, me avise e eu tento de outra forma! 😉\nSeu sorriso merece o melhor!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Mensagem de fechamento (que incentiva continuidade)
    if (messages.length > 3 && Math.random() > 0.7) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Fique à vontade para me perguntar o que quiser!\nEstou aqui para te dar toda atenção do mundo! 🌎💬\n\nQual serviço você gostaria de saber mais? 😄",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Resposta padrão com emojis
    return {
      id: Date.now().toString(),
      sender: 'bot',
      content: "Estou aqui para te ajudar com tudo sobre nossos serviços odontológicos e de harmonização facial! 😊 Quer saber sobre valores, agendar uma consulta ou conhecer nossos diferenciais? É só me dizer! 💬",
      timestamp: new Date(),
      sentiment: 'neutral'
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
        showServicesInfo: humanizedResponse.showServicesInfo,
        showScheduleInfo: humanizedResponse.showScheduleInfo,
        showClinicInfo: humanizedResponse.showClinicInfo,
        sentiment: 'neutral' // o bot sempre tem sentimento neutro
      };
      
      setMessages(prev => [...prev, botMessage]);
      setIsTyping(false);
    }, Math.random() * 1000 + 500);
    
    setInput("");
  };

  // Renderiza os cartões de serviços de forma organizada
  const ServiceInfoCard = () => {
    return (
      <div className="my-3 p-3 bg-primary/5 rounded-md border border-primary/20">
        <h3 className="text-sm font-bold mb-2 text-primary">💎 Nossos Serviços</h3>
        
        <div className="mb-3">
          <h4 className="text-xs font-semibold mb-1">🦷 Odontologia</h4>
          <ul className="text-xs space-y-1">
            {services.dental.map((service, i) => (
              <li key={i} className="flex justify-between">
                <span>{service.name}</span>
                <span className="font-semibold">R$ {service.price.toFixed(2)}</span>
              </li>
            ))}
          </ul>
        </div>
        
        <div>
          <h4 className="text-xs font-semibold mb-1">✨ Harmonização Facial</h4>
          <ul className="text-xs space-y-1">
            {services.harmonization.map((service, i) => (
              <li key={i} className="flex justify-between">
                <span>{service.name}</span>
                <span className="font-semibold">R$ {service.price.toFixed(2)}</span>
              </li>
            ))}
          </ul>
        </div>
      </div>
    );
  };

  // Renderiza informações de agendamento
  const ScheduleInfoCard = () => {
    return (
      <div className="my-3 p-3 bg-primary/5 rounded-md border border-primary/20">
        <div className="flex items-center gap-2 mb-2">
          <Calendar className="h-4 w-4 text-primary" />
          <h3 className="text-sm font-bold text-primary">Agendar Consulta</h3>
        </div>
        <p className="text-xs mb-2">
          Nossa avaliação inicial é <span className="font-bold">totalmente gratuita</span>! 
          Temos horários disponíveis:
        </p>
        <ul className="text-xs space-y-1 mb-2">
          <li className="flex justify-between">
            <span>• Segunda a sexta</span>
            <span>08:00 - 19:00</span>
          </li>
          <li className="flex justify-between">
            <span>• Sábados</span>
            <span>08:00 - 13:00</span>
          </li>
        </ul>
        <p className="text-xs italic">
          Responda com seu dia preferido e marcaremos sua consulta! 📅
        </p>
      </div>
    );
  };

  // Renderiza informações sobre a clínica
  const ClinicInfoCard = () => {
    return (
      <div className="my-3 p-3 bg-primary/5 rounded-md border border-primary/20">
        <div className="flex items-center gap-2 mb-2">
          <Info className="h-4 w-4 text-primary" />
          <h3 className="text-sm font-bold text-primary">Nossa Clínica</h3>
        </div>
        <p className="text-xs mb-2">
          Somos uma clínica especializada em:
        </p>
        <ul className="text-xs space-y-1">
          <li className="flex items-start gap-1">
            <ThumbsUp className="h-3 w-3 text-primary mt-0.5 flex-shrink-0" />
            <span>Profissionais com mais de 10 anos de experiência</span>
          </li>
          <li className="flex items-start gap-1">
            <ThumbsUp className="h-3 w-3 text-primary mt-0.5 flex-shrink-0" />
            <span>Equipamentos de última geração</span>
          </li>
          <li className="flex items-start gap-1">
            <ThumbsUp className="h-3 w-3 text-primary mt-0.5 flex-shrink-0" />
            <span>Protocolos rígidos de biossegurança</span>
          </li>
          <li className="flex items-start gap-1">
            <ThumbsUp className="h-3 w-3 text-primary mt-0.5 flex-shrink-0" />
            <span>Atendimento personalizado e humanizado</span>
          </li>
        </ul>
      </div>
    );
  };

  return (
    <>
      <Button
        onClick={() => setIsOpen(!isOpen)}
        size="icon"
        className="fixed bottom-6 right-6 h-12 w-12 rounded-full shadow-lg bg-gradient-to-br from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700"
      >
        <MessageSquare className="h-6 w-6" />
      </Button>

      {isOpen && (
        <Card className="fixed bottom-20 right-6 w-80 sm:w-96 shadow-lg z-50 border-blue-100">
          <CardHeader className="pb-2 bg-gradient-to-r from-blue-500 to-blue-600 text-white rounded-t-md">
            <CardTitle className="text-lg flex items-center gap-2">
              <Bot className="h-5 w-5 text-white" />
              <span>Assistente DentalSpa</span>
            </CardTitle>
          </CardHeader>
          <ScrollArea className="h-[400px] px-4" type="always">
            <CardContent className="space-y-4 pt-4">
              {messages.map((message) => (
                <div
                  key={message.id}
                  className={`flex ${message.sender === 'user' ? 'justify-end' : 'justify-start'}`}
                >
                  <div
                    className={`max-w-[85%] rounded-lg px-3 py-2 ${
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
                    <p className="text-sm whitespace-pre-line">{message.content}</p>
                    
                    {/* Cartões de informações especiais */}
                    {message.showServicesInfo && <ServiceInfoCard />}
                    {message.showScheduleInfo && <ScheduleInfoCard />}
                    {message.showClinicInfo && <ClinicInfoCard />}
                    
                    {/* Cupom de desconto */}
                    {message.hasCoupon && (
                      <div className="mt-2 p-2 bg-yellow-100 rounded-md border border-yellow-300">
                        <div className="flex items-center gap-2 mb-1">
                          <Gift className="h-3 w-3 text-yellow-700" />
                          <span className="text-xs font-medium text-yellow-700">Cupom de Desconto</span>
                        </div>
                        <p className="text-sm font-bold text-center text-yellow-700">SORRIA10</p>
                        <p className="text-xs text-center text-yellow-700">10% de desconto no próximo atendimento</p>
                      </div>
                    )}
                    
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
                      <MoreHorizontal className="h-4 w-4 animate-pulse" />
                      <span className="text-sm">Digitando...</span>
                    </div>
                  </div>
                </div>
              )}
              
              <div ref={messagesEndRef} />
            </CardContent>
          </ScrollArea>
          <CardFooter className="border-t p-3 bg-gray-50">
            <div className="flex w-full items-center space-x-2">
              <Input
                placeholder="Digite sua mensagem..."
                value={input}
                onChange={(e) => setInput(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSendMessage()}
                disabled={isTyping}
                className="border-blue-200 focus-visible:ring-blue-500"
              />
              <Button 
                size="icon" 
                onClick={handleSendMessage}
                disabled={isTyping}
                className="bg-gradient-to-r from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700"
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
