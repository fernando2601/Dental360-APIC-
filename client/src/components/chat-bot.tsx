import { useState, useRef, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageSquare, Send, User, Bot, Gift, MoreHorizontal, Calendar, Info, ThumbsUp } from "lucide-react";
import { useToast } from "@/hooks/use-toast";
import { formatTimeAgo } from "@/lib/utils";

// Definição dos diferentes tipos de fluxos de trabalho
type WorkflowType = 
  | 'initial' 
  | 'priceInquiry' 
  | 'serviceDetails' 
  | 'schedulingProcess' 
  | 'fearAndAnxiety' 
  | 'financialConcerns'
  | 'aestheticConcerns'
  | 'emergencyCase'
  | 'followUp';

// Interface para rastrear o estado do fluxo de trabalho
interface WorkflowState {
  type: WorkflowType;
  step: number;
  data: {
    service?: string;
    price?: number;
    preferredDate?: string;
    preferredTime?: string;
    concernType?: string;
    urgencyLevel?: 'low' | 'medium' | 'high';
    followUpDate?: Date;
    [key: string]: any; // Para dados adicionais específicos do fluxo
  };
}

// Tipagem para as mensagens
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
  // Campos adicionais para o workflow
  workflowAction?: string;
  workflowOptions?: string[];
  workflowType?: WorkflowType;
  expectsInput?: boolean;
  isWorkflowStep?: boolean;
};

// Componente de Chatbot
export function ChatBot() {
  const [input, setInput] = useState("");
  const [messages, setMessages] = useState<Message[]>([]);
  const [isTyping, setIsTyping] = useState(false);
  const [workflow, setWorkflow] = useState<WorkflowState | null>(null);
  const [lastInteractionTime, setLastInteractionTime] = useState(Date.now());

  const messagesEndRef = useRef<HTMLDivElement>(null);
  const { toast } = useToast();

  // Efeito para inicializar o chatbot com uma mensagem de boas-vindas
  useEffect(() => {
    // Adiciona a mensagem de boas-vindas inicial
    if (messages.length === 0) {
      setMessages([
        {
          id: "welcome-message",
          sender: "bot",
          content: "Olá! 😊 Bem-vindo à nossa clínica de harmonização e odontologia. Como você está hoje?",
          timestamp: new Date(),
          sentiment: "neutral"
        }
      ]);
    }
  }, [messages]);

  // Rola para a mensagem mais recente sempre que novas mensagens são adicionadas
  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }, [messages]);

  // Sistema de palavras-chave para respostas
  // Mapeamento de palavras-chave para respostas específicas
  const keywordResponses: Record<string, string[]> = {
    // Palavras relacionadas a dentes
    "dente": [
      "Seu sorriso merece o melhor cuidado! 😁 Somos especialistas em transformar sorrisos. Podemos agendar uma avaliação sem compromisso?",
      "Cuidar dos dentes é investir em saúde e autoestima! ✨ Temos as técnicas mais modernas para garantir seu conforto. Posso te contar mais?",
      "Problemas nos dentes? Relaxa, temos profissionais premiados prontos para cuidar de você! Que tal conhecer nossos tratamentos?"
    ],
    "dental": [
      "Estética dental é nossa especialidade! ✨ Transformamos sorrisos com técnicas de ponta. Quer saber mais sobre nossos tratamentos?",
      "Estamos entre os melhores em tratamentos dentais da região! 🏆 Mais de 500 pacientes satisfeitos todo mês. Quer fazer parte dessa estatística?"
    ],
    "sorriso": [
      "Um sorriso bonito abre portas! 😁 E nossos especialistas são mestres em criar sorrisos perfeitos. Posso te mostrar alguns antes/depois?",
      "Seu sorriso é seu cartão de visitas! ✨ Nossas técnicas avançadas garantem resultados incríveis. Que tal uma avaliação gratuita?",
      "Transformar sorrisos é nossa paixão! 💫 Já ajudamos mais de 10.000 pessoas a sorrirem com confiança. Podemos te ajudar também!"
    ],
    "branco": [
      "Dentes brancos e brilhantes em poucas sessões! ✨ Nosso clareamento possui tecnologia de ponta com resultados imediatos. Temos uma promoção especial hoje!",
      "Quem não quer aquele sorriso branquinho? 😁 Nosso tratamento é rápido, seguro e com resultados incríveis! Aproveite nossa oferta especial!"
    ],
    "siso": [
      "Extração de siso sem sofrimento? 😌 Aqui na clínica você encontra! Nossos especialistas usam técnicas modernas que tornam o procedimento mais confortável.",
      "Siso dando trabalho? Temos os melhores especialistas para cuidar disso! Procedimento rápido, seguro e com pós-operatório tranquilo."
    ],
    "harmonização": [
      "Nossa harmonização facial é referência! 💎 Resultados naturais que realçam sua beleza sem exageros. Quer conhecer nosso portfólio?",
      "Harmonização facial de qualidade faz toda diferença! ✨ Nossos profissionais são premiados e especialistas em resultados naturais."
    ],
    "botox": [
      "Botox aplicado por especialistas certificados! 🏆 Técnica precisa e resultados que valorizam sua expressão natural. Temos horários esta semana!",
      "Rejuvenescer com naturalidade é possível! 😊 Nosso botox tem a medida certa para suavizar linhas sem congelar expressões."
    ],
    "canal": [
      "Canal hoje em dia é procedimento tranquilo! 😌 Técnicas modernas e anestesia de qualidade para você não sentir nada.",
      "Dor de dente pode ser sinal que precisa de canal! Mas calma, na nossa clínica é procedimento rápido e sem sofrimento."
    ],
    "aparelho": [
      "Temos todas as opções de aparelhos! 😁 Desde os tradicionais até os invisíveis. Transforme seu sorriso com tecnologia avançada!",
      "Aparelho não precisa ser desconfortável! Nossas opções modernas são discretas e super eficientes."
    ],
    "limpeza": [
      "Limpeza profissional é fundamental! ✨ Remove manchas e previne problemas. Nosso ultrassom de última geração faz toda diferença!",
      "Uma boa limpeza deixa os dentes branquinhos na hora! E o melhor, evita problemas futuros mais sérios. Vamos agendar?"
    ]
  };

  // Inicia um fluxo de trabalho específico
  const startWorkflow = (type: WorkflowType, initialData: Record<string, any> = {}) => {
    setWorkflow({
      type,
      step: 0,
      data: { ...initialData }
    });
    
    // Adiciona a primeira mensagem do fluxo
    let firstMessage: Message = {
      id: Date.now().toString(),
      sender: 'bot',
      content: 'Vamos iniciar um novo atendimento!',
      timestamp: new Date(),
      sentiment: 'neutral',
      workflowType: type,
      isWorkflowStep: true
    };
    
    switch (type) {
      case 'priceInquiry':
        firstMessage.content = 'Sobre qual procedimento você gostaria de saber o preço?';
        firstMessage.expectsInput = true;
        break;
      case 'serviceDetails':
        firstMessage.content = 'Qual serviço você gostaria de conhecer melhor?';
        firstMessage.expectsInput = true;
        break;
      case 'schedulingProcess':
        firstMessage.content = 'Vamos agendar sua consulta! Qual procedimento você deseja realizar?';
        firstMessage.expectsInput = true;
        break;
      case 'fearAndAnxiety':
        firstMessage.content = 'Entendo sua preocupação. Muitas pessoas têm ansiedade com procedimentos odontológicos. Pode me contar mais sobre o que te deixa apreensivo?';
        firstMessage.expectsInput = true;
        break;
      case 'financialConcerns':
        firstMessage.content = 'Temos várias opções para facilitar seu tratamento. Qual procedimento você está considerando fazer?';
        firstMessage.expectsInput = true;
        break;
      case 'aestheticConcerns':
        firstMessage.content = 'Conte-me o que você gostaria de melhorar no seu sorriso:';
        firstMessage.expectsInput = true;
        break;
      case 'emergencyCase':
        firstMessage.content = 'Estamos prontos para te ajudar! Qual é a urgência que você está enfrentando?';
        firstMessage.expectsInput = true;
        break;
      default:
        firstMessage.content = 'Como posso ajudar você hoje?';
        firstMessage.expectsInput = true;
    }
    
    setMessages(prev => [...prev, firstMessage]);
  };

  // Processa respostas no contexto de um fluxo de trabalho ativo
  const processWorkflowResponse = (userText: string): Message | null => {
    if (!workflow) return null;
    
    const lowerText = userText.toLowerCase();
    
    switch (workflow.type) {
      case 'priceInquiry':
        // Fluxo de consulta de preços
        if (workflow.step === 0) {
          // Armazena o serviço consultado
          const service = userText;
          setWorkflow(prev => prev ? { ...prev, step: 1, data: { ...prev.data, service } } : null);
          
          // Identifica o preço baseado no serviço mencionado
          let price = 0;
          let confidence = 0;
          
          if (lowerText.includes("clareamento") || lowerText.includes("branqueamento")) {
            price = 800;
            confidence = 95;
          } 
          else if (lowerText.includes("siso") || lowerText.includes("juízo") || lowerText.includes("extrair dente")) {
            price = 450;
            confidence = 90;
          }
          else if (lowerText.includes("canal")) {
            price = 950;
            confidence = 95;
          }
          else if (lowerText.includes("limpeza") || lowerText.includes("profilaxia")) {
            price = 250;
            confidence = 95;
          }
          else if (lowerText.includes("aparelho")) {
            price = 2500;
            confidence = 80;
          }
          else if (lowerText.includes("botox") || lowerText.includes("toxina botulínica")) {
            price = 1200;
            confidence = 95;
          }
          else if (lowerText.includes("harmonização") || lowerText.includes("harmonizacao") || lowerText.includes("preenchedores")) {
            price = 3000;
            confidence = 90;
          }
          else if (lowerText.includes("faceta") || lowerText.includes("lente")) {
            price = 1500;
            confidence = 90;
          }
          else if (lowerText.includes("restauração") || lowerText.includes("restauracao") || lowerText.includes("obturação")) {
            price = 350;
            confidence = 90;
          }
          else {
            price = 500;
            confidence = 60;
          }
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Para ${service}, o investimento é em torno de R$${price.toFixed(2)}.\n\nEsse valor pode variar dependendo da sua necessidade específica após uma avaliação. Que tal agendar uma consulta sem compromisso para um orçamento personalizado?`,
            timestamp: new Date(),
            confidence: confidence,
            sentiment: 'neutral',
            workflowType: 'priceInquiry',
            isWorkflowStep: true
          };
        }
        return null;
        
      case 'serviceDetails':
        // Fluxo de detalhes de serviço
        if (workflow.step === 0) {
          // Identifica o serviço
          const service = userText;
          setWorkflow(prev => prev ? { ...prev, step: 1, data: { ...prev.data, service } } : null);
          
          if (lowerText.includes("clareamento") || lowerText.includes("branqueamento")) {
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: "Nosso clareamento é feito com tecnologia de ponta e gel de alta concentração, ativado por luz LED. O procedimento dura cerca de 1 hora e você já sai com resultado visível! Os dentes ficam até 8 tons mais claros.\n\nIncluímos também uma moldeira personalizada para manutenção em casa, o que prolonga muito o resultado!\n\nTemos um super desconto para quem agenda essa semana. Quer aproveitar?",
              timestamp: new Date(),
              sentiment: 'neutral',
              workflowType: 'serviceDetails',
              isWorkflowStep: true
            };
          }
          else if (lowerText.includes("siso") || lowerText.includes("juízo") || lowerText.includes("extrair dente")) {
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: "A extração de siso é realizada por nossos cirurgiões especialistas, com anestesia de qualidade e técnicas modernas que reduzem o trauma e o desconforto pós-operatório.\n\nO procedimento dura em média 30 minutos por dente, e utilizamos materiais que ajudam na cicatrização mais rápida.\n\nFornecemos todas as orientações pós-operatórias e medicação analgésica e anti-inflamatória para garantir sua recuperação tranquila.\n\nPodemos agendar sua avaliação?",
              timestamp: new Date(),
              sentiment: 'neutral',
              workflowType: 'serviceDetails',
              isWorkflowStep: true
            };
          }
          else if (lowerText.includes("canal")) {
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: "Nosso tratamento de canal é realizado com instrumentos automatizados que tornam o procedimento muito mais rápido e confortável que antigamente!\n\nUsamos anestesia potente e técnicas que garantem que você não sinta dor. Na maioria dos casos, conseguimos finalizar em apenas uma sessão.\n\nDe acordo com sua necessidade, já fazemos a restauração definitiva, deixando o dente protegido e funcional.\n\nQuer agendar uma avaliação para vermos seu caso específico?",
              timestamp: new Date(),
              sentiment: 'neutral',
              workflowType: 'serviceDetails',
              isWorkflowStep: true
            };
          }
          else {
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Nosso procedimento de ${service} é realizado com técnicas avançadas e materiais de primeira linha.\n\nNossos profissionais são especializados e possuem vasta experiência, garantindo resultados excepcionais com o máximo de conforto para você.\n\nQuer agendar uma avaliação para conhecer mais detalhes e receber um plano personalizado?`,
              timestamp: new Date(),
              sentiment: 'neutral',
              workflowType: 'serviceDetails',
              isWorkflowStep: true
            };
          }
        }
        return null;
        
      case 'schedulingProcess':
        // Implementação do fluxo de agendamento
        if (workflow.step === 0) {
          const service = userText;
          setWorkflow(prev => prev ? { ...prev, step: 1, data: { ...prev.data, service } } : null);
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Ótima escolha! Para ${service}, temos disponibilidade nos próximos dias. Qual seria sua preferência de data?`,
            timestamp: new Date(),
            sentiment: 'neutral',
            workflowType: 'schedulingProcess',
            isWorkflowStep: true,
            expectsInput: true
          };
        }
        else if (workflow.step === 1) {
          const preferredDate = userText;
          setWorkflow(prev => prev ? { ...prev, step: 2, data: { ...prev.data, preferredDate } } : null);
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Perfeito! Para o dia ${preferredDate}, temos horários pela manhã (8h-12h) e tarde (14h-18h). Qual horário seria melhor para você?`,
            timestamp: new Date(),
            sentiment: 'neutral',
            workflowType: 'schedulingProcess',
            isWorkflowStep: true,
            expectsInput: true
          };
        }
        else if (workflow.step === 2) {
          const preferredTime = userText;
          setWorkflow(prev => prev ? { ...prev, step: 3, data: { ...prev.data, preferredTime } } : null);
          
          const service = workflow.data.service || "";
          const date = workflow.data.preferredDate || "";
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Excelente! Vou reservar para você:\n\n• Procedimento: ${service}\n• Data: ${date}\n• Horário: ${preferredTime}\n\nNosso consultório está localizado na Av. Paulista, 1578 - Bela Vista.\n\nPoderia me informar seu nome completo e telefone para confirmarmos o agendamento?`,
            timestamp: new Date(),
            sentiment: 'neutral',
            workflowType: 'schedulingProcess',
            isWorkflowStep: true,
            expectsInput: true,
            showScheduleInfo: true
          };
        }
        else if (workflow.step === 3) {
          // Reseta o workflow após finalizar
          setWorkflow(null);
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: "Agendamento concluído com sucesso! Acabei de enviar uma confirmação para seu telefone com todos os detalhes.\n\nLembre-se de chegar 10 minutos antes para o preenchimento da ficha. Estamos ansiosos para cuidar do seu sorriso!\n\nPosso ajudar com mais alguma coisa?",
            timestamp: new Date(),
            sentiment: 'neutral'
          };
        }
        return null;
        
      // Outros casos de fluxo podem ser adicionados aqui
      case 'fearAndAnxiety':
        if (workflow.step === 0) {
          const concernType = userText;
          setWorkflow(prev => prev ? { ...prev, step: 1, data: { ...prev.data, concernType } } : null);
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: "Entendo completamente sua preocupação, e você não está sozinho(a). Muitos dos nossos pacientes chegam com receios semelhantes.\n\nNossa clínica é especializada em atendimento humanizado e acolhedor. Utilizamos anestesia de alta qualidade, com aplicação indolor e técnicas que respeitam seu tempo e conforto.\n\nPodemos começar com uma simples conversa na clínica, sem nenhum procedimento, apenas para você conhecer nossa equipe e se sentir mais confortável. O que acha?",
            timestamp: new Date(),
            sentiment: 'neutral',
            workflowType: 'fearAndAnxiety',
            isWorkflowStep: true
          };
        }
        return null;
        
      default:
          startWorkflow('initial', {});
          return null;
    }
  };

  // Gera respostas humanizadas com base no script e no sentimento
  const generateHumanizedResponse = (userText: string, sentiment: 'positive' | 'negative' | 'neutral'): Message => {
    const lowerText = userText.toLowerCase();
    
    // Atualiza o tempo da última interação
    setLastInteractionTime(Date.now());
    
    // Log para depuração
    console.log("Texto recebido:", userText);
    console.log("Texto convertido para minúsculo:", lowerText);
    
    // Primeiro verifica se estamos em um fluxo de trabalho ativo
    const workflowResponse = processWorkflowResponse(userText);
    if (workflowResponse) {
      return workflowResponse;
    }
    
    // Verifica palavras-chave isoladas antes de tudo
    for (const keyword in keywordResponses) {
      if (lowerText.includes(keyword)) {
        console.log(`Palavra-chave isolada encontrada: ${keyword}`);
        // Escolhe aleatoriamente uma das respostas disponíveis para essa palavra-chave
        const responses = keywordResponses[keyword];
        const randomIndex = Math.floor(Math.random() * responses.length);
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: responses[randomIndex],
          timestamp: new Date(),
          sentiment: 'neutral'
        };
      }
    }
    
    // Respostas específicas para perguntas sobre procedimentos dentários
    // Novas respostas adicionadas conforme solicitado pelo cliente
    if (lowerText.includes("siso") || lowerText.includes("juízo") || lowerText.includes("juizo")) {
      if (lowerText.includes("tirar") || lowerText.includes("arrancar") || lowerText.includes("extrair") || lowerText.includes("extraí")) {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Claro! E olha, tirar o siso com a gente é super tranquilo, viu? Temos técnicas modernas que deixam o procedimento rápido e confortável. Quer que eu te passe uma oferta especial para extração hoje? 😁",
          timestamp: new Date(),
          sentiment: 'neutral'
        };
      }
      
      if (lowerText.includes("cara inchada") || lowerText.includes("inchaço") || lowerText.includes("inchado")) {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Provavelmente é o siso, sim! 😬 Mas não se preocupe, a gente faz a avaliação e resolve isso com todo cuidado pra você sair aliviado(a)!",
          timestamp: new Date(),
          sentiment: 'neutral'
        };
      }
    }
    
    // Resposta específica para "Meu dente tá podre" e variações
    if (lowerText === "meu dente tá podre" || lowerText === "meu dente ta podre" || 
        lowerText.includes("meu dente tá podre") || lowerText.includes("meu dente ta podre") ||
        ((lowerText.includes("dente") || lowerText.includes("dental")) && 
        (lowerText.includes("podre") || lowerText.includes("estragado") || lowerText.includes("tá podre") || lowerText.includes("ta podre")))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Fica tranquilo(a)! Nós somos especialistas em salvar sorrisos! ❤️ Dá pra restaurar ou até reconstruir o dente, dependendo do caso. Vamos agendar uma avaliação sem compromisso?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("consertar") && lowerText.includes("sorriso")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Dá SIM e vai ficar incrível! ✨ Trabalhamos com estética dental de última geração para devolver a confiança no seu sorriso. Vamos marcar um horário para ver o que combina mais com você?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("dente quebrado") || (lowerText.includes("dente") && lowerText.includes("quebrou"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Calma, estamos aqui pra te ajudar! 🛟 Conseguimos restaurar o dente rapidinho e deixar seu sorriso novinho em folha! Quer que eu veja o melhor horário pra te encaixar hoje mesmo?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if ((lowerText.includes("sorriso") && lowerText.includes("branc")) || lowerText.includes("clareamento")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Já pensou sair com aquele sorriso de revista? 📸 A gente faz clareamento profissional seguro e com resultados incríveis. E hoje tem oferta especial, quer aproveitar?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if ((lowerText.includes("dói") || lowerText.includes("doi") || lowerText.includes("dor")) && lowerText.includes("arrancar")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Pode ficar tranquilo(a)! Usamos anestesia potente e técnicas modernas pra você nem sentir! 💤 A maioria dos pacientes até se surpreende de tão tranquilo que é. Vamos agendar?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("dente") && (lowerText.includes("doendo") || lowerText.includes("dói") || lowerText.includes("doi") || lowerText.includes("dor"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Dor de dente ninguém merece! 😢 Vamos cuidar de você com todo carinho, sem sofrimento. Nossa prioridade é acabar com essa dor o mais rápido possível. Que tal um atendimento prioritário?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Verifica se é uma resposta sobre como o usuário está se sentindo (logo após a primeira mensagem)
    if (messages.length === 1 || 
        (messages[messages.length-2]?.content.includes("Como você está hoje?") && 
         messages[messages.length-1].sender === 'user')) {
      
      if (sentiment === 'positive') {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Que alegria! 😍 Vamos deixar seu sorriso ainda mais incrível para combinar com esse seu astral!\n\nNosso clareamento dental é o MAIS PROCURADO do momento! Quer saber como funciona? Ou prefere conhecer nossos tratamentos de harmonização facial? Estamos com promoções IMPERDÍVEIS! 🤩",
          timestamp: new Date(),
          sentiment: 'neutral',
          hasCoupon: false
        };
      } else if (sentiment === 'negative') {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Poxa, sinto muito que não esteja em um bom dia. 💙 Que tal investir um pouco em você? Um sorriso mais bonito ou uma harmonização facial pode trazer um pouco mais de alegria para seus dias. A avaliação é TOTALMENTE gratuita e sem compromisso! 💬\n\nE tenho um cupom especial de 15% para você em qualquer tratamento. É nossa forma de tentar deixar seu dia um pouquinho melhor!",
          timestamp: new Date(),
          sentiment: 'neutral',
          hasCoupon: true
        };
      } else {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Ótimo! 😊 Posso ajudar com informações sobre nossos serviços odontológicos ou estéticos. O que você gostaria de saber? Temos desde limpezas e clareamentos até harmonização facial completa!",
          timestamp: new Date(),
          sentiment: 'neutral'
        };
      }
    }
    
    // Se nenhuma correspondência específica foi encontrada e não estamos em um fluxo,
    // retornamos uma resposta padrão de atendimento focada em vendas
    const defaultResponses = [
      "Adoramos sua mensagem! 🌟 Somos a clínica mais bem avaliada da região e estamos prontos para cuidar do seu sorriso com excelência. Podemos te ajudar com algum tratamento específico?",
      "Olá! Estamos aqui para tornar seu sorriso ainda mais bonito! 😁 Nossa equipe premiada tem transformado milhares de vidas. Em que podemos te ajudar hoje?",
      "Que bom te ver aqui! 💎 Nossa clínica é referência em tratamentos de alta qualidade. Conte-nos mais sobre o que você procura e vamos encontrar a solução perfeita para você!",
      "Seja bem-vindo(a)! 🏆 Somos especialistas em criar sorrisos de sonho. Nossos profissionais são mestres em soluções estéticas e odontológicas. Tem algum tratamento em mente?",
      "Obrigado pelo contato! 😊 Estamos prontos para te ajudar a conquistar o sorriso dos seus sonhos. Que tal agendar uma avaliação sem compromisso? Temos horários especiais esta semana!"
    ];
    
    const randomIndex = Math.floor(Math.random() * defaultResponses.length);
    return {
      id: Date.now().toString(),
      sender: 'bot',
      content: defaultResponses[randomIndex],
      timestamp: new Date(),
      sentiment: 'neutral'
    };
  };

  // Função para detectar o sentimento do texto
  const detectSentiment = (text: string): 'positive' | 'negative' | 'neutral' => {
    const lowerText = text.toLowerCase();
    
    // Palavras positivas em português
    const positiveWords = [
      "feliz", "bom", "ótimo", "excelente", "maravilhoso", "incrível", "fantástico",
      "alegre", "animado", "satisfeito", "contente", "adorei", "gostei", "legal", 
      "bacana", "top", "show", "perfeito", "sensacional", "massa", "demais", "lindo",
      "maravilha", "adoro", "amo", "gratidão", "obrigado", "obrigada", "amei", "gosto",
      "bem", "felicidade", "alegria", "positivo", "esperança", "motivado", "empolgado",
      "animada", "animado", "satisfeita", "satisfeito", "tranquilo", "tranquila", "calmo",
      "calma", "paz", "relaxado", "relaxada", "sorte", "sortuda", "sortudo"
    ];
    
    // Palavras negativas em português
    const negativeWords = [
      "triste", "ruim", "péssimo", "terrível", "horrível", "raiva", "ódio", "detesto",
      "irritado", "irritada", "chateado", "chateada", "frustrado", "frustrada", "medo",
      "receio", "ansioso", "ansiosa", "preocupado", "preocupada", "nervoso", "nervosa",
      "estressado", "estressada", "infeliz", "insatisfeito", "insatisfeita", "decepcionado",
      "decepcionada", "aborrecido", "aborrecida", "mal", "pior", "droga", "merda", "porcaria",
      "feio", "feia", "desanimado", "desanimada", "desmotivado", "desmotivada", "carregado",
      "tensa", "tenso", "deprimido", "deprimida", "angustiado", "angustiada", "aflito",
      "aflita", "dor", "sofrimento", "desespero", "desesperado", "desesperada", "difícil"
    ];
    
    let positiveScore = 0;
    let negativeScore = 0;
    
    // Analisa palavras positivas
    for (const word of positiveWords) {
      if (lowerText.includes(word)) {
        positiveScore++;
      }
    }
    
    // Analisa palavras negativas
    for (const word of negativeWords) {
      if (lowerText.includes(word)) {
        negativeScore++;
      }
    }
    
    // Verifica se há negação que inverte o sentimento
    const negationWords = ["não", "nem", "nunca", "jamais", "nenhum", "nenhuma"];
    for (const negation of negationWords) {
      // Se houver negação próxima a palavras positivas, reduz o score positivo
      for (const word of positiveWords) {
        if (lowerText.includes(`${negation} ${word}`) || 
            lowerText.includes(`${negation} é ${word}`) || 
            lowerText.includes(`${negation} está ${word}`)) {
          positiveScore--;
          negativeScore++;
        }
      }
      
      // Se houver negação próxima a palavras negativas, reduz o score negativo
      for (const word of negativeWords) {
        if (lowerText.includes(`${negation} ${word}`) || 
            lowerText.includes(`${negation} é ${word}`) || 
            lowerText.includes(`${negation} está ${word}`)) {
          negativeScore--;
          positiveScore++;
        }
      }
    }
    
    // Determina o sentimento com base nos scores
    if (positiveScore > negativeScore && positiveScore > 0) {
      return 'positive';
    } else if (negativeScore > positiveScore && negativeScore > 0) {
      return 'negative';
    } else {
      return 'neutral';
    }
  };

  // Envia a mensagem e simula uma resposta do chatbot
  const handleSendMessage = () => {
    if (!input.trim()) return;
    
    // Adiciona a mensagem do usuário
    const userMessage: Message = {
      id: Date.now().toString(),
      sender: 'user',
      content: input,
      timestamp: new Date(),
      sentiment: 'neutral'
    };
    
    setMessages(prev => [...prev, userMessage]);
    setInput("");
    
    // Detecta o sentimento do texto
    const sentiment = detectSentiment(input);
    
    // Simula o chatbot digitando
    setIsTyping(true);
    
    // Gera a resposta do chatbot após um pequeno delay
    setTimeout(() => {
      const botResponse = generateHumanizedResponse(input, sentiment);
      setMessages(prev => [...prev, botResponse]);
      setIsTyping(false);
    }, 1500);
  };

  // Renderização do componente
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
                
                <div className="whitespace-pre-wrap">{message.content}</div>
                
                {/* Componentes condicionais se necessário */}
                
                {/* Cupom de desconto */}
                {message.hasCoupon && (
                  <div className="mt-2 p-2 bg-yellow-100 rounded-md border border-yellow-300">
                    <div className="flex items-center gap-2 mb-1">
                      <Gift className="h-3 w-3 text-yellow-700" />
                      <span className="text-xs font-medium text-yellow-700">Cupom de Desconto</span>
                    </div>
                    <p className="text-sm font-bold text-center text-yellow-700">SORRIA15</p>
                    <p className="text-xs text-center text-yellow-700">15% de desconto no próximo atendimento</p>
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