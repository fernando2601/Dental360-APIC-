import { useState, useRef, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageSquare, X, Maximize2, Minimize2, Send, User, Bot } from "lucide-react";
import { formatTimeAgo } from "@/lib/utils";

// Tipagem para as mensagens
type Message = {
  id: string;
  sender: 'user' | 'bot';
  content: string;
  timestamp: Date;
};

// Respostas pré-definidas
const RESPONSES = {
  greeting: "Olá! Seja MUITO bem-vindo(a) à nossa clínica ✨\nEu sou o assistente virtual mais animado do Brasil! 😁\nComo você está hoje?",
  positive: "Que alegria! 😍 Vamos deixar seu sorriso ainda mais incrível!\nPosso te ajudar a encontrar o serviço ideal?",
  negative: "Poxa, sinto muito por isso. 😔\nPara melhorar seu dia, aqui vai um presente especial 🎁:\n**CUPOM DE DESCONTO DE 15%** para qualquer procedimento hoje!\n\nQuer que eu te ajude a agendar seu horário? 💬",
  services: "Esses são alguns dos nossos procedimentos mais procurados! 💖\n\n**Dentista:**\n• Limpeza: R$ 120\n• Clareamento: R$ 400\n• Tratamento de cárie: R$ 250\n• Aparelho ortodôntico (manutenção): R$ 180\n• Implante dentário: R$ 1.800\n\n**Harmonização Facial:**\n• Botox: R$ 500\n• Preenchimento labial: R$ 650\n• Bichectomia: R$ 1.200\n• Lifting facial com fios de PDO: R$ 2.000\n• Bioestimulador de colágeno: R$ 800\n\nPosso te passar mais detalhes sobre qualquer um deles! 👩‍⚕️👨‍⚕️",
  schedule: "Gostaria de agendar uma avaliação gratuita? 📅\nTemos horários incríveis essa semana!\nPosso ver qual o melhor para você?",
  doubt: "Sem problema! Estou aqui para te ajudar com calma! 🫶\nSe eu não expliquei direito, me avise e eu tento de outra forma! 😉\nSeu sorriso merece o melhor!",
  advantages: "Por que escolher a nossa clínica? 😍\n\n✨ Profissionais premiados e apaixonados pelo que fazem\n✨ Atendimento acolhedor e humanizado\n✨ Equipamentos modernos para seu conforto e segurança\n✨ Resultados naturais e personalizados para você!\n\nAqui você não é só mais um paciente, você é parte da nossa família 💖",
  closing: "Fique à vontade para me perguntar o que quiser!\nEstou aqui para te dar toda atenção do mundo! 🌎💬\n\nQual serviço você gostaria de saber mais? 😄",
  payment: "Sim, oferecemos pagamento em até 12x sem juros no cartão! 💳\nTambém aceitamos PIX, dinheiro e todos os cartões de débito.\nNossa prioridade é tornar seu tratamento acessível e confortável para você! 😊",
  siso: "Claro! E olha, tirar o siso com a gente é super tranquilo, viu? 😁\nTemos técnicas modernas que deixam o procedimento rápido e confortável.\n\nO valor da extração é R$ 250 por dente, e dá para parcelar em até 10x sem juros!\n\nQuer agendar uma avaliação gratuita?",
  clareamento: "Já pensou sair com aquele sorriso de revista? 📸\nA gente faz clareamento profissional seguro e com resultados incríveis! Seu sorriso pode ficar até 5 tons mais branco!\n\nO valor é R$ 400 e hoje temos uma oferta especial com 10% de desconto! Quer aproveitar?",
  bruxismo: "O bruxismo é mais comum do que você imagina! 😉\nTemos protetores bucais personalizados que vão proteger seus dentes e aliviar a tensão.\n\nO valor do protetor é R$ 200 e inclui as consultas de ajuste. Quer mais informações ou já podemos agendar?",
  default: "Estou aqui para te ajudar com qualquer dúvida sobre tratamentos dentários ou de harmonização! 😊\nQuer informações sobre algum procedimento específico ou prefere agendar uma avaliação gratuita?"
};

// Palavras-chave e suas respostas
const KEYWORDS: Record<string, string> = {
  // Dentistas
  "siso": RESPONSES.siso,
  "juízo": RESPONSES.siso,
  "clareamento": RESPONSES.clareamento,
  "branqueamento": RESPONSES.clareamento,
  "branquinho": RESPONSES.clareamento,
  "canal": "Fazer canal hoje em dia é super tranquilo! 😌\nUsamos técnicas modernas para garantir seu conforto. O tratamento de canal custa R$ 500 e pode ser parcelado. Quando podemos agendar para você?",
  "limpeza": "Uma limpeza profissional deixa seu sorriso muito mais bonito e saudável! ✨\nO procedimento custa R$ 120 e dura aproximadamente 40 minutos. Quer marcar para essa semana?",
  "aparelho": "Temos diversas opções de aparelhos ortodônticos! 😁\nDesde os tradicionais até os mais discretos. A manutenção mensal custa R$ 180. Podemos agendar uma avaliação gratuita para ver a melhor opção para você!",
  "invisível": "Sim, trabalhamos com alinhadores invisíveis! 👌\nSão discretos, confortáveis e removíveis! O valor do tratamento completo começa em R$ 4.500, parcelado em até 12x sem juros. Quer saber se é indicado para o seu caso?",
  "bruxismo": RESPONSES.bruxismo,
  "ranger": RESPONSES.bruxismo,
  "sensibilidade": "Entendo sua preocupação com a sensibilidade dental! 😔\nTemos tratamentos específicos que aliviam esse desconforto. Custa R$ 180 e o resultado é imediato! Gostaria de agendar?",
  "cárie": "Podemos tratar suas cáries com restaurações da cor do dente, super naturais! 😉\nO valor da restauração simples é R$ 250. E o melhor: sem dor! Quando podemos agendar?",
  "implante": "Os implantes dentários são a melhor solução para substituir dentes perdidos! 🦷\nSão feitos de titânio e parecem totalmente naturais. O valor do implante é R$ 1.800, parcelado em até 12x. Quer uma avaliação?",
  "extração": "Nossa equipe é especializada em extrações com o mínimo de desconforto! 👨‍⚕️\nUsamos anestesia de última geração para seu conforto. O valor varia de R$ 200 a R$ 350, dependendo da complexidade. Podemos agendar?",
  
  // Harmonização Facial
  "botox": "Nosso Botox é aplicado com técnica que garante expressões naturais! 💉✨\nO procedimento custa R$ 500 por região e o efeito dura em média 6 meses. Quer agendar uma avaliação gratuita?",
  "preenchimento": "O preenchimento labial deixa seus lábios mais volumosos e definidos! 💋\nUsamos ácido hialurônico de alta qualidade e o efeito dura cerca de 1 ano. O valor é R$ 650. Interessada?",
  "facial": "Nossa harmonização facial é personalizada para valorizar seus traços naturais! 👄✨\nO valor varia conforme as áreas tratadas, começando em R$ 800. Podemos fazer uma avaliação gratuita para criar um plano para você?",
  "bichectomia": "A bichectomia afia o contorno do rosto, destacando as maçãs do rosto! 😍\nO procedimento custa R$ 1.200 e tem resultados permanentes. Quer saber mais detalhes?",
  "papada": "Temos tratamentos específicos para papada, como aplicação de enzimas e tecnologias não invasivas! 👍\nO valor começa em R$ 600 por sessão. Quer conhecer as opções disponíveis para você?",
  
  // Pagamentos e preços
  "preço": RESPONSES.services,
  "valor": RESPONSES.services,
  "custa": RESPONSES.services,
  "cartão": RESPONSES.payment,
  "parcela": RESPONSES.payment,
  "débito": RESPONSES.payment,
  "pagamento": RESPONSES.payment,
  "pix": RESPONSES.payment,
  "dinheiro": RESPONSES.payment,
  
  // Sentimentos
  "bem": RESPONSES.positive,
  "feliz": RESPONSES.positive,
  "ótimo": RESPONSES.positive,
  "ótima": RESPONSES.positive,
  "mal": RESPONSES.negative,
  "triste": RESPONSES.negative,
  "péssimo": RESPONSES.negative,
  "péssima": RESPONSES.negative,
  "ruim": RESPONSES.negative,
  "cansado": RESPONSES.negative,
  "cansada": RESPONSES.negative,
  
  // Diferencial
  "por que": RESPONSES.advantages,
  "vantagem": RESPONSES.advantages,
  "diferencial": RESPONSES.advantages,
  "melhor": RESPONSES.advantages,
  
  // Dúvidas e traumas
  "dor": "Fique tranquilo(a), trabalhamos com anestesia moderna e técnicas suaves! 😌\nNosso objetivo é zero desconforto durante os procedimentos. Podemos agendar uma consulta para você conhecer nossa abordagem?",
  "medo": "Muitos pacientes chegam com medo e saem surpresos com a tranquilidade do atendimento! 🤗\nNossa equipe é treinada para oferecer um atendimento acolhedor e sem pressão. Quando podemos receber você?",
  "trauma": "Entendemos totalmente! 💕\nNossa clínica é especializada em atender pacientes com trauma de dentista. Vamos no seu ritmo, com muito carinho e paciência. Quer dar uma chance para nós?",
  
  // Agendamento
  "agendar": RESPONSES.schedule,
  "marcar": RESPONSES.schedule,
  "consulta": RESPONSES.schedule,
  "horário": RESPONSES.schedule,
  "avaliação": RESPONSES.schedule
};

// Componente de Chatbot
export function ChatBot() {
  const [isOpen, setIsOpen] = useState(false);
  const [isMinimized, setIsMinimized] = useState(false);
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
          content: RESPONSES.greeting,
          timestamp: new Date(),
        }
      ]);
    }
  }, []);

  // Rola para a mensagem mais recente
  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [messages]);

  // Função para gerar resposta baseada em palavras-chave
  const generateResponse = (message: string): string => {
    const lowerMessage = message.toLowerCase();
    
    // Verifica perda familiar
    if (lowerMessage.includes("perdi") && (
        lowerMessage.includes("mãe") || 
        lowerMessage.includes("pai") || 
        lowerMessage.includes("filho") || 
        lowerMessage.includes("filha") || 
        lowerMessage.includes("familiar") || 
        lowerMessage.includes("faleceu") || 
        lowerMessage.includes("falecimento") || 
        lowerMessage.includes("morreu")
      )) {
      return "Sinto muito pela sua perda! 💔 Momentos difíceis como esse nos lembram de cuidar de nós mesmos. Para ajudar nesse momento, queremos oferecer um cupom especial de 15% de desconto em qualquer tratamento. Podemos te ajudar de alguma forma?";
    }
    
    // Procura por palavras-chave específicas nas frases predefinidas
    for (const phrase of [
      "Preciso tirar o dente do juízo",
      "Meu dente tá podre",
      "Tenho que arrancar o siso",
      "Dá pra consertar meu sorriso",
      "Tô com o dente quebrado",
      "Quero deixar o sorriso branquinho"
    ]) {
      if (lowerMessage.includes(phrase.toLowerCase())) {
        // Encontra a resposta correspondente
        if (phrase.includes("juízo") || phrase.includes("siso")) {
          return RESPONSES.siso;
        } else if (phrase.includes("branquinho") || phrase.includes("branco")) {
          return RESPONSES.clareamento;
        } else if (phrase.includes("quebrado")) {
          return "Calma, estamos aqui pra te ajudar! 🛟 Conseguimos restaurar o dente rapidinho e deixar seu sorriso novinho em folha! Quer que eu veja o melhor horário pra te encaixar hoje mesmo?";
        } else if (phrase.includes("podre")) {
          return "Fica tranquilo(a)! Nós somos especialistas em salvar sorrisos! ❤️ Dá pra restaurar ou até reconstruir o dente, dependendo do caso. Vamos agendar uma avaliação sem compromisso?";
        } else if (phrase.includes("consertar") || phrase.includes("sorriso")) {
          return "Dá SIM e vai ficar incrível! ✨ Trabalhamos com estética dental de última geração para devolver a confiança no seu sorriso. Vamos marcar um horário para ver o que combina mais com você?";
        }
      }
    }
    
    // Checa por palavras-chave individuais
    for (const [keyword, response] of Object.entries(KEYWORDS)) {
      if (lowerMessage.includes(keyword)) {
        return response;
      }
    }
    
    return RESPONSES.default;
  };

  // Envia a mensagem e gera uma resposta inteligente
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
    
    // Determina o tempo de digitação baseado no tamanho da resposta
    const response = generateResponse(input);
    const typingTime = Math.min(2000, 500 + response.length * 5);
    
    // Gera a resposta após um pequeno delay
    setTimeout(() => {
      const botResponse: Message = {
        id: Date.now().toString(),
        sender: 'bot',
        content: response,
        timestamp: new Date()
      };
      
      setMessages(prev => [...prev, botResponse]);
      setIsTyping(false);
    }, typingTime);
  };

  // Toggle para abrir/fechar o chat
  const toggleChat = () => {
    setIsOpen(!isOpen);
    if (!isOpen) {
      setIsMinimized(false);
    }
  };

  // Toggle para minimizar/maximizar o chat
  const toggleMinimize = (e: React.MouseEvent) => {
    e.stopPropagation();
    setIsMinimized(!isMinimized);
  };

  return (
    <div className="fixed bottom-4 right-4 z-50 flex flex-col items-end">
      {/* Botão para abrir o chat */}
      {!isOpen && (
        <Button 
          onClick={toggleChat}
          size="lg"
          className="rounded-full p-4 bg-primary hover:bg-primary/90 shadow-lg"
        >
          <MessageSquare className="h-6 w-6" />
          <span className="ml-2">Chat</span>
        </Button>
      )}
      
      {/* Janela do chat */}
      {isOpen && (
        <Card className="w-80 md:w-96 shadow-xl transition-all duration-300">
          <CardHeader className="border-b p-3 flex flex-row justify-between items-center">
            <CardTitle className="text-sm flex items-center gap-2">
              <MessageSquare className="h-4 w-4" />
              Assistente Virtual DentalSpa
            </CardTitle>
            <div className="flex space-x-1">
              <Button 
                variant="ghost" 
                size="icon" 
                className="h-7 w-7"
                onClick={toggleMinimize}
              >
                {isMinimized ? <Maximize2 className="h-4 w-4" /> : <Minimize2 className="h-4 w-4" />}
              </Button>
              <Button 
                variant="ghost" 
                size="icon" 
                className="h-7 w-7"
                onClick={toggleChat}
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
          </CardHeader>
          
          {!isMinimized && (
            <>
              <CardContent className="p-0">
                <ScrollArea className="h-[350px] p-3">
                  {messages.map((message) => (
                    <div
                      key={message.id}
                      className={`flex ${
                        message.sender === 'user' ? 'justify-end' : 'justify-start'
                      } mb-3`}
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
                        
                        <div className="whitespace-pre-wrap text-sm">{message.content}</div>
                        
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
                    className="flex-1 text-sm"
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
            </>
          )}
        </Card>
      )}
    </div>
  );
}