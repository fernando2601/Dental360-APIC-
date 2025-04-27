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

export function ChatBot() {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState<Message[]>([
    {
      id: "welcome",
      sender: "bot",
      content: "Olá! Seja MUITO bem-vindo(a) à nossa clínica ✨\nEu sou o assistente virtual mais animado do Brasil! 😁\nComo você está hoje?",
      timestamp: new Date(),
      workflowType: 'initial',
      isWorkflowStep: true,
      expectsInput: true
    }
  ]);
  const [input, setInput] = useState("");
  const { toast } = useToast();
  const messagesEndRef = useRef<HTMLDivElement>(null);

  // Estado para controlar se está "digitando"
  const [isTyping, setIsTyping] = useState(false);
  
  // Estado para rastrear o fluxo de trabalho atual
  const [currentWorkflow, setCurrentWorkflow] = useState<WorkflowState>({
    type: 'initial',
    step: 0,
    data: {}
  });
  
  // Função para atualizar o fluxo de trabalho
  const updateWorkflow = (updates: Partial<WorkflowState>) => {
    setCurrentWorkflow(prev => ({
      ...prev,
      ...updates,
      data: {
        ...prev.data,
        ...(updates.data || {})
      }
    }));
  };
  
  // Função para avançar para o próximo passo no fluxo
  const advanceWorkflowStep = () => {
    setCurrentWorkflow(prev => ({
      ...prev,
      step: prev.step + 1
    }));
  };
  
  // Função para iniciar um novo fluxo de trabalho
  const startWorkflow = (type: WorkflowType, initialData: Record<string, any> = {}) => {
    setCurrentWorkflow({
      type,
      step: 0,
      data: initialData
    });
  };
  
  // Serviços e preços
  const services = {
    dental: [
      { name: "Limpeza Dental", price: 120, description: "Remoção completa de placa bacteriana e tártaro." },
      { name: "Clareamento Dental", price: 400, description: "Deixe seu sorriso até 8 tons mais branco!" },
      { name: "Tratamento de Cárie", price: 250, description: "Restauração com materiais de última geração." },
      { name: "Aparelho Ortodôntico", price: 180, description: "Manutenção mensal de aparelhos." },
      { name: "Implante Dentário", price: 1800, description: "Substitui dentes perdidos com raiz artificial." },
      { name: "Extração de Siso", price: 450, description: "Procedimento indolor com técnicas modernas." },
      { name: "Tratamento de Gengivite", price: 280, description: "Combate a inflamação gengival antes que se agrave." },
      { name: "Facetas Dentárias", price: 900, description: "Corrige forma e cor dos dentes com porcelana." },
      { name: "Restauração Estética", price: 200, description: "Repara dentes danificados por cáries ou fraturas." },
      { name: "Tratamento de Canal", price: 700, description: "Procedimento para salvar dentes comprometidos." }
    ],
    harmonization: [
      { name: "Botox", price: 500, description: "Suaviza rugas e linhas de expressão sem cirurgia." },
      { name: "Preenchimento Labial", price: 650, description: "Volumiza e define os lábios para aparência mais jovem." },
      { name: "Bichectomia", price: 1200, description: "Afina o rosto removendo as bolas de Bichat." },
      { name: "Lifting Facial", price: 2000, description: "Rejuvenescimento facial com fios de PDO." },
      { name: "Bioestimulador de Colágeno", price: 800, description: "Estimula produção natural de colágeno para pele mais firme." },
      { name: "Preenchimento Facial", price: 1300, description: "Restaura volume em áreas com perda de gordura." },
      { name: "Harmonização Facial", price: 3500, description: "Conjunto de procedimentos para equilíbrio facial." },
      { name: "Rinomodelação", price: 1700, description: "Harmoniza o nariz sem cirurgia." }
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
    
    // Situações traumáticas ou muito tristes (prioridade máxima)
    if (lowerText.includes("morreu") || lowerText.includes("faleceu") || lowerText.includes("morte") || 
        lowerText.includes("falecimento") || lowerText.includes("perdeu") || lowerText.includes("perdi") ||
        lowerText.includes("luto") || lowerText.includes("tragédia") || lowerText.includes("acidente grave") ||
        lowerText.includes("hospital") || lowerText.includes("doença") || lowerText.includes("câncer") ||
        lowerText.includes("terminal") || (lowerText.includes("irmão") && (lowerText.includes("morreu") || lowerText.includes("faleceu")))) {
      return 'negative';
    }
    
    // Palavras que indicam sentimento negativo ou tristeza
    const negativeWords = ['triste', 'chateado', 'frustrado', 'infeliz', 'preocupado', 'dor', 'sofr', 'caro', 'custa', 'preço', 'magoado', 
    'ansioso', 'ansiedade', 'medo', 'assustado', 'desapontado', 'decepcionado', 'angustiado', 'não gosto', 'não quero', 
    'ruim', 'péssimo', 'horrível', 'mal', 'pior', 'não estou bem', 'doente', 'cansado', 'estressado', 'sozinho',
    'sofrendo', 'difícil', 'problema', 'complicado', 'depressão', 'deprimido', 'acabado'];
    
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
  
  // Variável para rastrear quando foi a última interação do usuário
  const [lastInteractionTime, setLastInteractionTime] = useState<number>(Date.now());
  
  // Verifica se o usuário está inativo há mais de 5 minutos
  const isUserInactive = () => {
    return Date.now() - lastInteractionTime > 5 * 60 * 1000; // 5 minutos em milissegundos
  };
  
  // Processa a resposta com base no fluxo de trabalho atual
  const processWorkflowResponse = (userText: string): Message | null => {
    const lowerText = userText.toLowerCase();
    
    // Se não estiver em um fluxo de trabalho ou estiver no fluxo inicial passo 0, retorne null para seguir o fluxo normal
    if (currentWorkflow.type === 'initial' && currentWorkflow.step === 0) {
      return null;
    }
    
    // Fluxo de Agendamento
    if (currentWorkflow.type === 'schedulingProcess') {
      switch (currentWorkflow.step) {
        // Passo 1: Coletando o serviço desejado
        case 0:
          // Tenta identificar o serviço mencionado
          const dentServices = services.dental.map(s => s.name.toLowerCase());
          const harmServices = services.harmonization.map(s => s.name.toLowerCase());
          const allServices = [...dentServices, ...harmServices];
          
          let matchedService = '';
          
          // Verifica se algum serviço foi mencionado
          for (const service of allServices) {
            if (lowerText.includes(service.toLowerCase())) {
              matchedService = service;
              break;
            }
          }
          
          // Se identificou um serviço
          if (matchedService) {
            // Atualiza os dados do workflow
            updateWorkflow({
              step: 1,
              data: { service: matchedService }
            });
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Ótima escolha! O serviço de ${matchedService} é um dos nossos mais procurados! 🌟\n\nQual seria a melhor data para você? Temos horários disponíveis esta semana!`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: true
            };
          } 
          // Se não identificou um serviço específico
          else {
            // Oferece opções 
            const dentalOptions = services.dental.slice(0, 3).map(s => s.name);
            const harmOptions = services.harmonization.slice(0, 2).map(s => s.name);
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Para agendar, preciso saber qual serviço você deseja. Aqui estão algumas opções populares:\n\n**Odontologia:**\n• ${dentalOptions.join('\n• ')}\n\n**Harmonização:**\n• ${harmOptions.join('\n• ')}\n\nQual destes serviços você gostaria de agendar? Ou me diga se deseja outro serviço.`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: true,
              workflowOptions: [...dentalOptions, ...harmOptions]
            };
          }
          
        // Passo 2: Coletando a data preferida  
        case 1:
          // Extrai informações de data do texto
          let dateInfo = '';
          
          if (lowerText.includes('segunda') || lowerText.includes('segunda-feira')) {
            dateInfo = 'segunda-feira';
          } else if (lowerText.includes('terça') || lowerText.includes('terça-feira')) {
            dateInfo = 'terça-feira';
          } else if (lowerText.includes('quarta') || lowerText.includes('quarta-feira')) {
            dateInfo = 'quarta-feira';
          } else if (lowerText.includes('quinta') || lowerText.includes('quinta-feira')) {
            dateInfo = 'quinta-feira';
          } else if (lowerText.includes('sexta') || lowerText.includes('sexta-feira')) {
            dateInfo = 'sexta-feira';
          } else if (lowerText.includes('sábado') || lowerText.includes('sabado')) {
            dateInfo = 'sábado';
          } else if (lowerText.includes('amanhã') || lowerText.includes('amanha')) {
            dateInfo = 'amanhã';
          } else if (lowerText.includes('hoje')) {
            dateInfo = 'hoje';
          } else if (lowerText.includes('próxima semana') || lowerText.includes('proxima semana')) {
            dateInfo = 'próxima semana';
          } else if (lowerText.includes('esse final de semana') || lowerText.includes('este final de semana')) {
            dateInfo = 'este final de semana';
          } else {
            // Tenta extrair datas no formato dd/mm ou números
            const dateRegex = /\d{1,2}\/\d{1,2}/;
            const dateMatch = lowerText.match(dateRegex);
            if (dateMatch) {
              dateInfo = dateMatch[0];
            }
          }
          
          // Se conseguiu extrair alguma data
          if (dateInfo) {
            updateWorkflow({
              step: 2,
              data: { preferredDate: dateInfo }
            });
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Perfeito! Tenho disponibilidade para ${dateInfo}. 📆\n\nQual horário seria melhor para você? Temos manhã (8h às 12h) ou tarde (13h às 18h).`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: true
            };
          } 
          // Se não conseguiu extrair uma data
          else {
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Não consegui identificar uma data específica. Que tal me dizer o dia da semana que prefere? Por exemplo: "segunda-feira", "terça à tarde", ou uma data como "15/05".`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: true
            };
          }
          
        // Passo 3: Coletando o horário preferido  
        case 2:
          // Extrai informações de horário do texto
          let timeInfo = '';
          
          if (lowerText.includes('manhã') || lowerText.includes('manha')) {
            timeInfo = 'manhã';
          } else if (lowerText.includes('tarde')) {
            timeInfo = 'tarde';
          } else if (lowerText.includes('noite')) {
            timeInfo = 'fim da tarde';
          } else {
            // Tenta extrair horários no formato hh:mm ou apenas hh
            const timeRegex = /\d{1,2}[h:]\d{0,2}/;
            const timeMatch = lowerText.match(timeRegex);
            if (timeMatch) {
              timeInfo = timeMatch[0];
            }
          }
          
          // Se conseguiu extrair algum horário
          if (timeInfo) {
            updateWorkflow({
              step: 3,
              data: { preferredTime: timeInfo }
            });
            
            // Confirma os dados coletados e finaliza o agendamento
            const { service, preferredDate } = currentWorkflow.data;
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `ÓTIMO! 🎉 Reservei seu horário para ${service} na ${preferredDate}, período da ${timeInfo}.\n\nUma pessoa da nossa equipe entrará em contato para confirmar o horário exato e passar todas as orientações.\n\nSua avaliação inicial é TOTALMENTE GRATUITA! Além disso, por ter agendado online, você receberá um KIT ESPECIAL de boas-vindas na primeira consulta! 🎁\n\nPosso ajudar com mais alguma coisa?`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: false
            };
          } 
          // Se não conseguiu extrair um horário
          else {
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Não consegui identificar um horário específico. Por favor, me diga se prefere "manhã", "tarde" ou um horário específico como "14h".`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: true
            };
          }
          
        default:
          // Reseta o workflow se chegou ao final
          startWorkflow('initial', {});
          return null;
      }
    }
    
    // Fluxo de Informações sobre Serviços (Preços)
    else if (currentWorkflow.type === 'priceInquiry') {
      switch (currentWorkflow.step) {
        // Passo 1: Detalhando o serviço de interesse e perguntando se deseja agendar
        case 0:
          const { service, price } = currentWorkflow.data;
          
          // Avança o workflow
          advanceWorkflowStep();
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Sobre o ${service}: o valor é **R$ ${(price || 0).toFixed(2)}** e temos diversas opções de pagamento para facilitar sua vida!\n\nNossos clientes AMAM os resultados desse procedimento. Temos mais de 95% de satisfação! 🤩\n\nGostaria de agendar uma avaliação GRATUITA para saber mais detalhes ou tirar dúvidas presencialmente?`,
            timestamp: new Date(),
            workflowType: 'priceInquiry',
            isWorkflowStep: true,
            expectsInput: true
          };
          
        // Passo 2: Verificando se quer agendar e redirecionando para o workflow de agendamento se sim
        case 1:
          // Verifica se a resposta é positiva para agendar
          const isPositive = lowerText.includes('sim') || lowerText.includes('quero') || 
                             lowerText.includes('claro') || lowerText.includes('ok') ||
                             lowerText.includes('pode') || lowerText.includes('gostaria') ||
                             lowerText.includes('vamos');
                             
          if (isPositive) {
            // Inicia o workflow de agendamento com os dados do serviço já preenchidos
            const { service, price } = currentWorkflow.data;
            startWorkflow('schedulingProcess', { service, price });
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Ótima escolha! 😊 Vamos agendar uma avaliação para o serviço de ${service}.\n\nQual seria a melhor data para você? Temos horários disponíveis ainda esta semana!`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: true
            };
          } else {
            // Se não deseja agendar, oferece outras opções
            startWorkflow('initial', {});
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Sem problemas! Estamos aqui sempre que precisar. 😊\n\nSe tiver outras dúvidas sobre nossos serviços, formas de pagamento ou qualquer outro assunto, é só me perguntar!`,
              timestamp: new Date(),
              workflowType: 'initial',
              isWorkflowStep: false,
              expectsInput: true
            };
          }
          
        default:
          // Reseta o workflow se chegou ao final
          startWorkflow('initial', {});
          return null;
      }
    }
    
    // Fluxo para Casos de Medo/Ansiedade
    else if (currentWorkflow.type === 'fearAndAnxiety') {
      switch (currentWorkflow.step) {
        // Passo 1: Oferecendo informações adicionais sobre como lidamos com medo
        case 0:
          // Avança o workflow
          advanceWorkflowStep();
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Entendo sua preocupação com o medo. 💙 Muitos dos nossos pacientes também tinham receio antes da primeira consulta.\n\nNossa clínica tem um protocolo especial para lidar com pacientes ansiosos, que inclui:\n\n• Ambiente calmo com música relaxante\n• Explicação detalhada de cada procedimento antes de iniciar\n• Pausas sempre que você precisar\n• Técnicas de anestesia indolor\n• Opção de sedação consciente para casos mais extremos\n\nGostaria de conhecer nossa clínica sem compromisso? Muitos pacientes relatam que apenas conhecer o ambiente já ajuda a reduzir a ansiedade.`,
            timestamp: new Date(),
            workflowType: 'fearAndAnxiety',
            isWorkflowStep: true,
            expectsInput: true
          };
          
        // Passo 2: Verificando se deseja agendar visita especial para conhecer a clínica
        case 1:
          // Verifica se a resposta é positiva
          const isPositive = lowerText.includes('sim') || lowerText.includes('quero') || 
                             lowerText.includes('claro') || lowerText.includes('ok') ||
                             lowerText.includes('pode') || lowerText.includes('gostaria') ||
                             lowerText.includes('vamos');
                             
          if (isPositive) {
            // Inicia o workflow de agendamento com o tipo especial de visita
            startWorkflow('schedulingProcess', { service: 'Visita de reconhecimento sem procedimentos' });
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Excelente escolha! 👏 Vamos agendar uma visita especial para você conhecer nossa clínica, sem nenhum procedimento.\n\nSerá apenas uma conversa com nossa equipe, para você se familiarizar com o ambiente e tirar todas as dúvidas. Muitos pacientes se sentem muito mais confiantes depois dessa primeira visita!\n\nQual seria a melhor data para você?`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: true
            };
          } else {
            // Se não deseja agendar, oferece outras opções
            startWorkflow('initial', {});
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Sem problemas! Quando se sentir confortável, estamos aqui. 😊\n\nSe preferir, posso enviar mais informações sobre nossas técnicas para lidar com medo e ansiedade, ou você pode ver depoimentos de pacientes que também tinham receio antes de nos conhecer. O que acha?`,
              timestamp: new Date(),
              workflowType: 'initial',
              isWorkflowStep: false,
              expectsInput: true
            };
          }
          
        default:
          // Reseta o workflow se chegou ao final
          startWorkflow('initial', {});
          return null;
      }
    }
    
    // Fluxo para Preocupações Estéticas
    else if (currentWorkflow.type === 'aestheticConcerns') {
      switch (currentWorkflow.step) {
        // Passo 1: Coletando informações sobre a preocupação específica do paciente
        case 0:
          // Identifica preocupações específicas mencionadas pelo usuário
          let concernTypeValue = '';
          
          if (lowerText.includes('dente') || lowerText.includes('dentes')) {
            if (lowerText.includes('torto') || lowerText.includes('tortos') || lowerText.includes('desalinhado')) {
              concernTypeValue = 'dentes desalinhados';
            } else if (lowerText.includes('amarelo') || lowerText.includes('amarelos') || lowerText.includes('escuro')) {
              concernTypeValue = 'dentes amarelados';
            } else if (lowerText.includes('quebrado') || lowerText.includes('quebrados') || lowerText.includes('rachado')) {
              concernTypeValue = 'dentes danificados';
            } else {
              concernTypeValue = 'problemas dentários';
            }
          } else if (lowerText.includes('ruga') || lowerText.includes('rugas') || lowerText.includes('idade')) {
            concernTypeValue = 'rugas e linhas de expressão';
          } else if (lowerText.includes('lábio') || lowerText.includes('labio') || lowerText.includes('boca')) {
            concernTypeValue = 'estética labial';
          } else if (lowerText.includes('nariz') || lowerText.includes('rinoplastia')) {
            concernTypeValue = 'estética nasal';
          } else if (lowerText.includes('queixo') || lowerText.includes('mandíbula') || lowerText.includes('mandibula')) {
            concernTypeValue = 'contorno mandibular';
          } else if (lowerText.includes('pele') || lowerText.includes('tez') || lowerText.includes('acne')) {
            concernTypeValue = 'textura da pele';
          } else {
            concernTypeValue = 'estética geral';
          }
          
          // Atualiza o workflow com os dados e avança para o próximo passo
          updateWorkflow({
            step: 1,
            data: { concernType: concernTypeValue }
          });
          
          let treatmentSuggestion;
          switch (concernTypeValue) {
            case 'dentes desalinhados':
              treatmentSuggestion = 'aparelho ortodôntico transparente, que é praticamente invisível';
              break;
            case 'dentes amarelados':
              treatmentSuggestion = 'clareamento dental profissional, que pode clarear até 8 tons';
              break;
            case 'dentes danificados':
              treatmentSuggestion = 'restaurações estéticas com resina ou facetas de porcelana';
              break;
            case 'rugas e linhas de expressão':
              treatmentSuggestion = 'aplicação de Botox ou bioestimuladores de colágeno';
              break;
            case 'estética labial':
              treatmentSuggestion = 'preenchimento labial com ácido hialurônico';
              break;
            case 'estética nasal':
              treatmentSuggestion = 'rinomodelação sem cirurgia';
              break;
            case 'contorno mandibular':
              treatmentSuggestion = 'harmonização facial com foco em definição mandibular';
              break;
            case 'textura da pele':
              treatmentSuggestion = 'tratamentos para revitalização da pele';
              break;
            default:
              treatmentSuggestion = 'harmonização facial personalizada';
          }
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Obrigado por compartilhar isso comigo! Entendo sua preocupação com ${concernTypeValue}.\n\nBoas notícias: temos tratamentos específicos para esse caso, como ${treatmentSuggestion}.\n\nMuitas pessoas se sentem da mesma forma, e o resultado após o tratamento não é apenas estético, mas também um aumento significativo da autoestima e qualidade de vida.\n\nGostaria de conhecer mais detalhes sobre esse tratamento específico ou agendar uma avaliação gratuita?`,
            timestamp: new Date(),
            workflowType: 'aestheticConcerns',
            isWorkflowStep: true,
            expectsInput: true
          };
          
        // Passo 2: Oferecendo opções de tratamento e verificando se deseja agendar
        case 1:
          const aestheticConcernType = currentWorkflow.data.concernType || '';
          const isPositiveResponse = lowerText.includes('sim') || lowerText.includes('quero') || 
                                     lowerText.includes('claro') || lowerText.includes('ok') ||
                                     lowerText.includes('gostaria') || lowerText.includes('agendar') ||
                                     lowerText.includes('avaliação') || lowerText.includes('consulta');
          
          if (isPositiveResponse) {
            // Inicia o workflow de agendamento com o tipo de preocupação
            startWorkflow('schedulingProcess', { service: `Avaliação para ${aestheticConcernType}` });
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Excelente! 🌟 Vamos agendar uma avaliação gratuita para conversarmos sobre o tratamento para ${aestheticConcernType}.\n\nNessa avaliação, nossos especialistas vão examinar seu caso específico e apresentar todas as opções de tratamento personalizadas para você, incluindo custos e tempo de tratamento.\n\nQual seria a melhor data para sua visita?`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: true
            };
          } else if (lowerText.includes('preço') || lowerText.includes('preco') || lowerText.includes('valor') || 
                     lowerText.includes('custo') || lowerText.includes('quanto custa') || lowerText.includes('investimento')) {
            
            // Redireciona para o workflow de preocupações financeiras
            startWorkflow('financialConcerns', { concernType: aestheticConcernType });
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Entendo que os valores são uma parte importante da sua decisão. Os tratamentos para ${aestheticConcernType} têm uma faixa de preço que varia conforme a complexidade do caso e técnicas utilizadas.\n\nO mais importante é que oferecemos diversas opções de pagamento para caber no seu orçamento!\n\n• Parcelamento em até 12x SEM JUROS\n• Descontos para pagamento à vista\n• Pacotes promocionais\n\nPara dar um valor exato, precisamos realizar uma avaliação. Mas posso te adiantar que o investimento inicial para este tipo de tratamento começa em aproximadamente R$ ${Math.floor(Math.random() * 400) + 200},00.\n\nGostaria de agendar uma avaliação gratuita para conhecer todas as opções e valores?`,
              timestamp: new Date(),
              workflowType: 'financialConcerns',
              isWorkflowStep: true,
              expectsInput: true
            };
          } else {
            // Se a resposta não for clara, oferece mais informações
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Entendo! Para ajudar em sua decisão, posso te contar que nossos tratamentos para ${aestheticConcernType} são:\n\n• Minimamente invasivos\n• Com resultados visíveis em poucas sessões\n• Realizados por profissionais especializados\n• Personalizados para cada paciente\n\nAlém disso, temos centenas de casos de sucesso com resultados extraordinários! Se preferir, podemos agendar apenas uma consulta informativa, sem compromisso, para você conhecer melhor as opções. O que acha?`,
              timestamp: new Date(),
              workflowType: 'aestheticConcerns',
              isWorkflowStep: true,
              expectsInput: true
            };
          }
          
        default:
          // Reseta o workflow se chegou ao final
          startWorkflow('initial', {});
          return null;
      }
    }
    
    // Fluxo para Preocupações Financeiras
    else if (currentWorkflow.type === 'financialConcerns') {
      switch (currentWorkflow.step) {
        // Passo 1: Identificando o serviço de interesse e apresentando opções de pagamento
        case 0:
          // Tenta identificar o serviço mencionado ou usa informações prévias
          let serviceOfInterestInitial = '';
          const allServices = [...services.dental.map(s => s.name.toLowerCase()), ...services.harmonization.map(s => s.name.toLowerCase())];
          
          for (const service of allServices) {
            if (lowerText.includes(service.toLowerCase())) {
              serviceOfInterestInitial = service;
              break;
            }
          }
          
          // Se não identificou nenhum serviço específico, verifica se há um tipo de preocupação anterior
          if (!serviceOfInterestInitial && currentWorkflow.data.concernType) {
            serviceOfInterestInitial = `tratamento para ${currentWorkflow.data.concernType}`;
          }
          
          // Se mesmo assim não tiver nada, usa um termo genérico
          if (!serviceOfInterestInitial) {
            serviceOfInterestInitial = 'nossos procedimentos';
          }
          
          // Atualiza o workflow e avança para o próximo passo
          updateWorkflow({
            step: 1,
            data: { serviceOfInterest: serviceOfInterestInitial }
          });
          
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Fico feliz que esteja considerando ${serviceOfInterestInitial}! 💯\n\nEntendo que o aspecto financeiro é importante, e por isso criamos opções flexíveis para todos os orçamentos:\n\n• Pagamento parcelado em até 12x sem juros (via cartão de crédito)\n• 5% de desconto para pagamento via PIX\n• 3% de desconto para pagamento em dinheiro\n• Pacotes com desconto progressivo (quanto mais sessões, maior o desconto)\n• Planos de tratamento customizados para caber no seu orçamento\n\nAlém disso, oferecemos avaliação TOTALMENTE GRATUITA para que você saiba exatamente os valores antes de iniciar qualquer procedimento.\n\nGostaria de agendar esta avaliação?`,
            timestamp: new Date(),
            workflowType: 'financialConcerns',
            isWorkflowStep: true,
            expectsInput: true
          };
          
        // Passo 2: Verificando se deseja agendar e redirecionando para workflow de agendamento ou oferecendo desconto
        case 1:
          const serviceOfInterestValue = currentWorkflow.data.serviceOfInterest || '';
          const wantsToSchedule = lowerText.includes('sim') || lowerText.includes('quero') || 
                                  lowerText.includes('claro') || lowerText.includes('ok') ||
                                  lowerText.includes('gostaria') || lowerText.includes('agendar') ||
                                  lowerText.includes('avaliação') || lowerText.includes('consulta');
          
          if (wantsToSchedule) {
            // Inicia o workflow de agendamento
            startWorkflow('schedulingProcess', { service: `Avaliação para ${serviceOfInterestValue}` });
            
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Excelente decisão! 🌟 Vamos agendar sua avaliação gratuita para ${serviceOfInterestValue}.\n\nNessa consulta, além de avaliarmos seu caso específico, apresentaremos todas as opções de pagamento detalhadas e personalizadas para o seu orçamento.\n\nQual seria a melhor data para você?`,
              timestamp: new Date(),
              workflowType: 'schedulingProcess',
              isWorkflowStep: true,
              expectsInput: true
            };
          } else if (lowerText.includes('caro') || lowerText.includes('muito') || lowerText.includes('não posso') || 
                     lowerText.includes('nao posso') || lowerText.includes('não tenho') || lowerText.includes('alto')) {
            
            // Se ainda está preocupado com o valor, oferece um desconto especial
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Entendo sua preocupação com os valores. 💙\n\nPara casos como o seu, temos uma condição ESPECIAL que posso oferecer: um CUPOM DE 10% DE DESCONTO que você pode usar na primeira sessão após a avaliação!\n\nAlém disso, ao agendar hoje, você garante os preços atuais que serão reajustados no próximo mês.\n\nMuitos pacientes se surpreendem quando descobrem que os valores ficam bem mais acessíveis do que imaginavam, especialmente com nossas opções de parcelamento.\n\nPodemos agendar uma avaliação sem compromisso apenas para você conhecer todas as opções e valores exatos?`,
              timestamp: new Date(),
              workflowType: 'financialConcerns',
              isWorkflowStep: true,
              expectsInput: true,
              hasCoupon: true
            };
          } else {
            // Se ainda está em dúvida, oferece mais informações
            return {
              id: Date.now().toString(),
              sender: 'bot',
              content: `Entendo! É importante você ter todas as informações necessárias para tomar a melhor decisão.\n\nSaiba que nossa clínica trabalha com transparência total nos valores e nunca existem "surpresas" ou taxas adicionais durante o tratamento.\n\nMuitos dos nossos pacientes relatam que, considerando os resultados obtidos e o impacto positivo na qualidade de vida, o investimento foi extremamente válido.\n\nEstou à disposição para esclarecer qualquer dúvida adicional sobre valores, formas de pagamento ou agendamento. O que mais você gostaria de saber?`,
              timestamp: new Date(),
              workflowType: 'financialConcerns',
              isWorkflowStep: true,
              expectsInput: true
            };
          }
          
        default:
          // Reseta o workflow se chegou ao final
          startWorkflow('initial', {});
          return null;
      }
    }
    
    // Se não corresponder a nenhum fluxo ativo ou estiver no fluxo inicial
    return null;
  };

  // Gera respostas humanizadas com base no script e no sentimento
  const generateHumanizedResponse = (userText: string, sentiment: 'positive' | 'negative' | 'neutral'): Message => {
    const lowerText = userText.toLowerCase();
    
    // Atualiza o tempo da última interação
    setLastInteractionTime(Date.now());
    
    // Verificação para respostas de alta prioridade
    // Tratamos primeiro casos especiais que precisam de resposta exata
    // Adicionamos um log para depuração
    console.log("Texto recebido:", userText);
    console.log("Texto convertido para minúsculo:", lowerText);
    
    if (lowerText === "meu dente tá podre" || lowerText === "meu dente ta podre") {
      console.log("MATCH EXATO: Detectado 'meu dente tá podre'");
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Fica tranquilo(a)! Nós somos especialistas em salvar sorrisos! ❤️ Dá pra restaurar ou até reconstruir o dente, dependendo do caso. Vamos agendar uma avaliação sem compromisso?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }

    // Primeiro verifica se estamos em um fluxo de trabalho ativo
    const workflowResponse = processWorkflowResponse(userText);
    if (workflowResponse) {
      return workflowResponse;
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
    
    if (lowerText.includes("latejando") || lowerText.includes("canal")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Pode ser mesmo, mas fique tranquilo(a)! 🙌 Fazer canal hoje em dia é simples e alivia muito! Agendamos rapidinho e ainda parcelamos o tratamento pra você não se preocupar.",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("canal") && (lowerText.includes("morto") || lowerText.includes("dente morto"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sim, é importante tratar! 😌 Nosso tratamento é rápido, moderno e confortável. Vamos salvar seu sorriso juntos? Aproveita que estamos com condições especiais hoje!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("aparelho") && (lowerText.includes("soltou") || lowerText.includes("quebrou"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sem estresse! 😅 A gente arruma pra você rapidinho! E ainda fazemos um check-up pra garantir que tá tudo certinho. Vamos agendar?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("gengiva") && (lowerText.includes("sangra") || lowerText.includes("sangrando"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Pode ser só uma limpeza que tá precisando, mas também pode ser sinal de gengivite. 😬 Melhor a gente ver direitinho. Agendamos uma consulta de avaliação para você?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if ((lowerText.includes("porcelana") || lowerText.includes("faceta")) && lowerText.includes("dente")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Perfeito! ✨ As facetas de porcelana são maravilhosas pra deixar seu sorriso lindo, natural e duradouro. E dá pra parcelar! Vamos agendar seu orçamento personalizado?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("dente") && (lowerText.includes("torto") || lowerText.includes("tortos") || lowerText.includes("arruma"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Com certeza! 🙌 Seja com aparelho tradicional ou alternativas mais discretas, a gente tem a solução ideal pra você. Bora transformar esse sorriso?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if ((lowerText.includes("sem aparelho") || lowerText.includes("aparelho não")) && (lowerText.includes("sorriso") || lowerText.includes("dente"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Dá sim! 😁 Trabalhamos com lentes de contato dental e outras técnicas estéticas que corrigem imperfeições sem precisar de aparelho. Vamos conversar?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("mau hálito") || lowerText.includes("mau halito") || lowerText.includes("bafo")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Pode ser algo simples como placa bacteriana ou gengivite. 😷 Mas fica tranquilo(a), que com uma limpeza e orientação correta a gente resolve! Vamos marcar uma avaliação?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("orçamento") && (lowerText.includes("gratis") || lowerText.includes("grátis") || lowerText.includes("gratuito"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Claro! 😃 A avaliação inicial é gratuita para você conhecer o que precisa e receber o melhor plano de tratamento. Quer que eu reserve um horário pra você?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("sem dor") || ((lowerText.includes("dói") || lowerText.includes("doi") || lowerText.includes("dor")) && lowerText.includes("não"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Tem sim! 😌 Trabalhamos com anestesia moderna e muita experiência pra garantir seu conforto. Nosso lema é cuidar de você sem sofrimento!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if ((lowerText.includes("trato") || lowerText.includes("cuidar")) && lowerText.includes("sorriso")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Show! 🚀 Cuidar do sorriso é investir em autoestima. Limpeza, clareamento, correções... montamos o plano perfeito pra você sair brilhando. Vamos começar?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("anestesia") && (lowerText.includes("medo") || lowerText.includes("receio") || lowerText.includes("pavor"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Relaxa! 😁 Nossa anestesia é segura e praticamente indolor. E nossa equipe é treinada pra te deixar super tranquilo(a). Seu conforto é nossa prioridade!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("dente") && (lowerText.includes("preto") || lowerText.includes("escuro") || lowerText.includes("escurecido"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Não se preocupe, sempre tem solução! 🙌 Pode ser uma restauração, uma limpeza profunda ou outro tratamento estético. Vamos cuidar disso juntos?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if ((lowerText.includes("lente") || lowerText.includes("lentes")) && (lowerText.includes("dente") || lowerText.includes("dental"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sim! As famosas lentes de contato dental! 😍 Seu sorriso vai ficar alinhado, branquíssimo e super natural. Vamos agendar uma avaliação sem compromisso?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("dente") && (lowerText.includes("mole") || lowerText.includes("mexendo"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Melhor vir avaliar! 👨‍⚕️ Dente mole pode ser gengiva, trauma ou outro fator. Cuidar cedo é fundamental pra salvar o dente!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("salvar") && lowerText.includes("dente")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sempre tentamos salvar o dente antes de qualquer outro procedimento! 🛡️ Tratamentos modernos tornam isso cada vez mais possível. Vamos avaliar?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if ((lowerText.includes("pagar") || lowerText.includes("pagamento")) && 
        (lowerText.includes("cartão") || lowerText.includes("cartao") || lowerText.includes("parcela"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sim, sem problemas! 😃 Facilitamos pra você cuidar do sorriso sem pesar no bolso. Até 10x no cartão em alguns casos! Quer que eu calcule pra você?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if ((lowerText.includes("branqueamento") || lowerText.includes("clareamento")) && lowerText.includes("rápido")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sim! 😁 Temos clareamento a laser que deixa o sorriso até 5x mais branco em poucas sessões! E hoje tem promoção, hein!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("tapa") && (lowerText.includes("visual") || lowerText.includes("sorriso"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Perfeito! 😍 Nada como um sorriso novo pra dar aquele upgrade! Harmonização facial, estética dental... vamos deixar você ainda mais incrível!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("sensíve") || lowerText.includes("sensivel") || lowerText.includes("água gelada")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sensibilidade é super comum! 😥 Vamos ver se é algo simples de resolver com limpeza, tratamento ou até produtos específicos. Agendamos?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("placa bacteriana") || (lowerText.includes("dente") && lowerText.includes("esquisito"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Placa a gente remove fácil com uma limpeza profissional! 😁 E você já sai do consultório sentindo a diferença. Quer reservar seu horário?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if ((lowerText.includes("dente") || lowerText.includes("dentes")) && (lowerText.includes("separado") || lowerText.includes("aberto") || lowerText.includes("espaço"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Dá sim! ✨ Muitas vezes conseguimos corrigir com facetas ou lentes de contato dental. Um sorriso alinhado pode estar mais perto do que você imagina!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("limpar") && lowerText.includes("clarear") && lowerText.includes("ajeitar")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Aí sim! 😍 E a gente ama cuidar de sorrisos completos! Montamos um pacote personalizado pra você sair daqui transformado(a)! Vamos montar o seu?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("sorriso") && lowerText.includes("feio")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Não existe sorriso feio, só sorriso que ainda não foi cuidado por nós! 😍 Vem com a gente que vamos deixar você amando seu espelho!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("trauma") && lowerText.includes("dentista")) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Mais que de boa! 🥰 Somos especialistas em atendimento humanizado. Sem pressão, com calma e muito carinho. Você vai se surpreender positivamente!",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    if (lowerText.includes("sorriso") && (lowerText.includes("artista") || lowerText.includes("famoso") || lowerText.includes("celebridade"))) {
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "E você merece! 🎬✨ Trabalhamos com estética de alto nível pra deixar seu sorriso digno de capa de revista! Vamos marcar uma avaliação VIP pra você?",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Verificação especial para situações de luto ou perda de familiar (prioridade máxima)
    if ((lowerText.includes("irmão") || lowerText.includes("irmã") || lowerText.includes("pai") || 
        lowerText.includes("mãe") || lowerText.includes("mãe") || lowerText.includes("filho") || 
        lowerText.includes("filha") || lowerText.includes("avó") || lowerText.includes("avô") || 
        lowerText.includes("tio") || lowerText.includes("tia") || lowerText.includes("primo") || 
        lowerText.includes("prima") || lowerText.includes("familiar") || lowerText.includes("parente")) && 
        (lowerText.includes("morreu") || lowerText.includes("faleceu") || lowerText.includes("perdi") || 
        lowerText.includes("morte") || lowerText.includes("perdeu") || lowerText.includes("luto"))) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sinto muito pela sua perda. 😔 Momentos como esse são realmente difíceis. Estamos aqui para oferecer todo apoio que precisar.\n\nComo um gesto de solidariedade, gostaria de oferecer um **CUPOM ESPECIAL DE 15% DE DESCONTO** em qualquer procedimento quando você sentir que é o momento adequado.\n\nNão há pressa. Quando estiver pronto, estamos aqui para ajudar a cuidar de você com todo carinho e atenção que merece. 💖",
        timestamp: new Date(),
        sentiment: 'neutral',
        hasCoupon: true
      };
    }
    
    // Verifica se o usuário está perguntando como o bot está
    if (lowerText.includes("como você está") || lowerText.includes("como voce esta") || 
        lowerText.includes("tudo bem com você") || lowerText.includes("e você") || lowerText.includes("e vc") ||
        lowerText.includes("e voce") || lowerText.includes("tudo bem contigo") || 
        lowerText.includes("como vai") || lowerText.includes("como vai você") || 
        (lowerText.includes("tudo") && lowerText.includes("bem"))) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Estou SUPER bem! 🤩 Muito animada para te atender hoje! E você, como está? Posso ajudar com algo específico? 😊",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Verifica se está perguntando sobre formas de pagamento
    if (lowerText.includes("paga") || lowerText.includes("pagamento") || lowerText.includes("dinheiro") || 
        lowerText.includes("cartão") || lowerText.includes("cartao") || lowerText.includes("débito") || 
        lowerText.includes("credito") || lowerText.includes("crédito") || lowerText.includes("pix") ||
        lowerText.includes("parcela")) {
      
      // Mensagem específica para cartão de crédito
      if (lowerText.includes("cartão") || lowerText.includes("cartao") || lowerText.includes("credito") || lowerText.includes("crédito")) {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Aceitamos cartão de crédito em até 12x SEM JUROS! 💳✨\n\nNão vai pesar NADA no seu bolso! E para quem fecha o pacote completo de tratamento, temos condições AINDA MAIS vantajosas!\n\nQuer agendar uma avaliação para conhecer todos os detalhes? Nossa agenda dessa semana está quase lotada! 📅",
          timestamp: new Date(),
          sentiment: 'neutral'
        };
      }
      
      // Mensagem específica para PIX
      if (lowerText.includes("pix")) {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Pagamento via PIX tem 5% de DESCONTO ESPECIAL! 🤑\n\nÉ nossa forma de pagamento preferida: rápida, segura e com vantagem extra para você!\n\nTemos também outras opções de pagamento disponíveis. Quer conhecer?",
          timestamp: new Date(),
          sentiment: 'neutral'
        };
      }
      
      // Mensagem específica para dinheiro
      if (lowerText.includes("dinheiro")) {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: "Pagamento em dinheiro tem 3% de desconto! 💵\n\nE claro, fornecemos recibo e nota fiscal para sua segurança e tranquilidade!\n\nTemos também outras formas de pagamento. Posso detalhar alguma específica para você?",
          timestamp: new Date(),
          sentiment: 'neutral'
        };
      }
      
      // Mensagem genérica sobre formas de pagamento (quando nenhuma específica foi mencionada)
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Temos TODAS as formas de pagamento para facilitar sua vida! 💳💰\n\n• PIX (com 5% de desconto! 🤑)\n• Cartão de crédito (em até 12x sem juros!)\n• Cartão de débito\n• Dinheiro (com 3% de desconto)\n\nFácil, né? E o melhor: para procedimentos acima de R$1.000, oferecemos condições SUPER especiais! Quer que te conte sobre nossos pacotes com descontos incríveis? 🎁",
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
          content: "Poxa, sinto muito por isso. 😔\nPara melhorar seu dia, aqui vai um presente especial 🎁:\n**CUPOM DE DESCONTO DE 15%** para qualquer procedimento hoje!\n\nSei que momentos difíceis são complicados, mas estamos aqui para te apoiar! Nossos tratamentos podem ajudar a trazer um pouco mais de alegria para seus dias. A avaliação é TOTALMENTE gratuita e sem compromisso! 💬",
          timestamp: new Date(),
          sentiment: 'neutral',
          hasCoupon: true
        };
      }
    }
    
    // Verifica se está perguntando sobre preços ou serviços
    if (lowerText.includes("preço") || lowerText.includes("valor") || lowerText.includes("custo") || 
        lowerText.includes("quanto custa") || lowerText.includes("serviço") || lowerText.includes("procedimento") ||
        lowerText.includes("tratamento") || lowerText.includes("fazer") || lowerText.includes("quanto tempo") ||
        lowerText.includes("dói") || lowerText.includes("doi") || lowerText.includes("doer") || 
        lowerText.includes("realizar") || lowerText.includes("especialista") || lowerText.includes("profissional")) {
      
      // Detectar se está perguntando sobre um serviço específico
      let specificService = "";
      let price = 0;
      
      // Serviços dentários - baseado na lista de palavras-chave fornecida
      if (lowerText.includes("limpeza") || lowerText.includes("profilaxia") || lowerText.includes("tártaro") || lowerText.includes("tartaro") || lowerText.includes("placa bacteriana")) {
        specificService = "limpeza dental";
        price = 120;
      }
      else if (lowerText.includes("clareamento") || lowerText.includes("dentes brancos") || lowerText.includes("clareamento a laser") || lowerText.includes("clareamento caseiro")) {
        specificService = "clareamento dental";
        price = 400;
      }
      else if (lowerText.includes("aparelho") || lowerText.includes("ortodon") || lowerText.includes("aparelho fixo") || 
               lowerText.includes("aparelho móvel") || lowerText.includes("dente torto") || lowerText.includes("invisível") || 
               lowerText.includes("invisivel") || lowerText.includes("alinhar os dentes") || lowerText.includes("mordida cruzada")) {
        specificService = "aparelho ortodôntico";
        price = 180;
      }
      else if (lowerText.includes("implante") || lowerText.includes("implante dentário") || lowerText.includes("colocar dente") || lowerText.includes("dente artificial")) {
        specificService = "implante dentário";
        price = 1800;
      }
      else if (lowerText.includes("siso") || lowerText.includes("dente do siso") || lowerText.includes("dentes do siso") || 
               lowerText.includes("dente siso") || lowerText.includes("tirar siso") || lowerText.includes("tirar o siso") || 
               lowerText.includes("arrancar siso") || lowerText.includes("siso nascendo") || lowerText.includes("siso está nascendo") || 
               lowerText.includes("dente incluso")) {
        specificService = "extração de siso";
        price = 450;
      }
      else if (lowerText.includes("extração") || lowerText.includes("extracao") || lowerText.includes("arrancar dente") || 
               lowerText.includes("tirar dente") || lowerText.includes("remover dente") || lowerText.includes("dente quebrou")) {
        specificService = "extração dentária";
        price = 450;
      }
      else if (lowerText.includes("gengiv") || lowerText.includes("periodont") || lowerText.includes("inflamação") || 
               lowerText.includes("inflamacao") || lowerText.includes("sangramento") || lowerText.includes("retração gengival") || 
               lowerText.includes("gengivite")) {
        specificService = "tratamento de gengivite";
        price = 280;
      }
      else if (lowerText.includes("faceta") || lowerText.includes("lente") || lowerText.includes("lente de contato dental") || 
               lowerText.includes("porcelana") || lowerText.includes("dente quebrado") || lowerText.includes("estética dental")) {
        specificService = "facetas dentárias";
        price = 900;
      }
      else if (lowerText.includes("canal") || lowerText.includes("endodontia") || lowerText.includes("nervo") || 
               lowerText.includes("polpa") || lowerText.includes("tratamento de canal") || lowerText.includes("pulpotomia")) {
        specificService = "tratamento de canal";
        price = 700;
      }
      else if (lowerText.includes("restaura") || lowerText.includes("obturação") || lowerText.includes("obturacao") || 
               lowerText.includes("cárie") || lowerText.includes("carie") || lowerText.includes("dente quebrado") || 
               lowerText.includes("resina")) {
        specificService = "restauração estética";
        price = 200;
      }
      else if (lowerText.includes("consulta") || lowerText.includes("avaliação") || lowerText.includes("avaliacao") || 
               lowerText.includes("check-up") || lowerText.includes("check up") || lowerText.includes("exame") || 
               lowerText.includes("diagnóstico") || lowerText.includes("diagnostico") || lowerText.includes("primeira vez")) {
        specificService = "consulta inicial com diagnóstico";
        price = 0; // Gratuita
      }
      else if (lowerText.includes("raio") || lowerText.includes("raio-x") || lowerText.includes("raio x") || 
               lowerText.includes("radiografia") || lowerText.includes("imagem") || lowerText.includes("panorâmica") || 
               lowerText.includes("panoramica")) {
        specificService = "radiografia odontológica";
        price = 80;
      }
      else if (lowerText.includes("sensibilidade") || lowerText.includes("dente sensível") || lowerText.includes("dente sensivel") || 
               lowerText.includes("dói com frio") || lowerText.includes("doi com frio") || lowerText.includes("dói com doce") || 
               lowerText.includes("doi com doce")) {
        specificService = "tratamento para sensibilidade dentária";
        price = 150;
      }
      else if (lowerText.includes("prótese") || lowerText.includes("protese") || lowerText.includes("dentadura") || 
               lowerText.includes("ponte") || lowerText.includes("coroa") || lowerText.includes("dente artificial")) {
        specificService = "prótese dentária";
        price = 950;
      }
      else if (lowerText.includes("bruxismo") || lowerText.includes("ranger") || lowerText.includes("protetor") || 
               lowerText.includes("protetor bucal") || lowerText.includes("placa")) {
        specificService = "tratamento para bruxismo";
        price = 350;
      }
      else if (lowerText.includes("halitose") || lowerText.includes("mau hálito") || lowerText.includes("mau halito") || 
               lowerText.includes("bafo") || lowerText.includes("cheiro ruim")) {
        specificService = "tratamento para halitose";
        price = 180;
      }
      else if (lowerText.includes("odontopediatria") || lowerText.includes("criança") || lowerText.includes("crianca") || 
               lowerText.includes("bebê") || lowerText.includes("bebe") || lowerText.includes("filho") || 
               lowerText.includes("filha") || lowerText.includes("infantil")) {
        specificService = "odontopediatria";
        price = 150;
      }
      else if (lowerText.includes("urgência") || lowerText.includes("urgencia") || lowerText.includes("emergência") || 
               lowerText.includes("emergencia") || lowerText.includes("dor forte") || lowerText.includes("acidente") || 
               lowerText.includes("quebrou agora")) {
        specificService = "atendimento de emergência";
        price = 200;
      }
      // Harmonização facial
      else if (lowerText.includes("botox") || lowerText.includes("toxina botulínica") || lowerText.includes("rugas")) {
        specificService = "aplicação de botox";
        price = 500;
      }
      else if (lowerText.includes("preenchimento") || lowerText.includes("labial") || lowerText.includes("ácido hialurônico") || 
               lowerText.includes("acido hialuronico") || lowerText.includes("volume")) {
        specificService = "preenchimento labial";
        price = 650;
      }
      else if (lowerText.includes("bichectomia") || lowerText.includes("bochecha") || lowerText.includes("afinar rosto")) {
        specificService = "bichectomia";
        price = 1200;
      }
      else if (lowerText.includes("lifting") || lowerText.includes("fios") || lowerText.includes("fio russo") || 
               lowerText.includes("fios de sustentação") || lowerText.includes("flacidez")) {
        specificService = "lifting facial";
        price = 2000;
      }
      else if (lowerText.includes("colágeno") || lowerText.includes("colageno") || lowerText.includes("bioestimulador")) {
        specificService = "bioestimulador de colágeno";
        price = 800;
      }
      else if (lowerText.includes("harmoniza") || lowerText.includes("facial") || lowerText.includes("harmonização facial") || 
               lowerText.includes("orofacial") || lowerText.includes("estética facial")) {
        specificService = "harmonização facial";
        price = 3500;
      };
      
      if (specificService) {
        // Respostas especiais para procedimentos específicos
        if (specificService === "extração de siso") {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `SIM, realizamos extração de siso! 😁 É um dos procedimentos mais procurados da nossa clínica!\n\nO valor é de **R$ ${price.toFixed(2)}** e temos condições especiais de pagamento.\n\nNossa equipe é ESPECIALISTA nesse procedimento, garantindo uma recuperação rápida e o mínimo de desconforto possível. Usamos anestesia de última geração para você não sentir NADA!\n\nQuer agendar uma avaliação? Posso verificar os horários disponíveis! ✨`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
        
        else if (specificService === "tratamento de canal") {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Realizamos tratamento de canal com MÁXIMO conforto! 🦷\n\nO valor é de **R$ ${price.toFixed(2)}** e pode ser parcelado.\n\nNossa técnica moderna garante um procedimento praticamente SEM DOR, diferente do que muitos imaginam! Usamos equipamentos de última geração e anestesia eficiente.\n\nNão deixe para depois! Agende uma avaliação e resolva o problema antes que piore. ✨`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
        
        else if (specificService === "clareamento dental") {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Nosso clareamento dental é REVOLUCIONÁRIO! ✨\n\nO valor é de **R$ ${price.toFixed(2)}** (temos opções a laser e caseiro).\n\nO resultado é IMEDIATO e pode clarear até 8 tons em uma única sessão! É seguro, não danifica o esmalte e tem efeito duradouro.\n\nEstamos com uma PROMOÇÃO especial essa semana! Quer garantir seu sorriso brilhante? 😁`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
        
        else if (specificService === "aparelho ortodôntico") {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Temos TODOS os tipos de aparelhos ortodônticos! 😍\n\nO valor inicial é de **R$ ${price.toFixed(2)}** por mês (varia conforme o tipo escolhido).\n\nDispomos de aparelhos convencionais, estéticos, autoligados e invisíveis - para todas as necessidades e bolsos!\n\nNossa equipe de ortodontistas é ESPECIALIZADA e vai criar um plano de tratamento personalizado para você. Agende uma avaliação GRATUITA! 🌟`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
        
        else if (specificService === "facetas dentárias") {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Transforme seu sorriso com nossas facetas dentárias premium! ✨\n\nO valor é de **R$ ${price.toFixed(2)}** por unidade, com condições especiais para pacotes.\n\nNossas lentes de contato dentais são ultrafinas, resistentes e IDÊNTICAS aos dentes naturais. O procedimento é rápido, indolor e o resultado é IMEDIATO!\n\nTemos um DESCONTO ESPECIAL para quem agendar a avaliação esta semana! 💎`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
        
        else if (specificService === "implante dentário") {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Nossos implantes dentários são referência de QUALIDADE! 👑\n\nO valor é de **R$ ${price.toFixed(2)}** por unidade, parcelado em até 12x.\n\nUtilizamos implantes de titânio da mais alta qualidade, com técnicas minimamente invasivas e rápida recuperação. O resultado é 100% natural e PERMANENTE!\n\nAgendando a avaliação hoje, você ganha a tomografia computadorizada GRATUITAMENTE! 🎁`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
        
        else if (specificService === "consulta inicial com diagnóstico") {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Nossa consulta inicial com diagnóstico completo é TOTALMENTE GRATUITA! ✨\n\nInclui avaliação detalhada, radiografia digital, plano de tratamento personalizado e orçamento sem compromisso.\n\nÉ uma oportunidade perfeita para conhecer nossa clínica e tirar todas as suas dúvidas com nossos especialistas.\n\nQual o melhor dia para você? Temos horários disponíveis ainda esta semana! 📅`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
        
        else if (specificService === "limpeza dental") {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Nossa limpeza dental profissional é COMPLETA! ✨\n\nO valor é de **R$ ${price.toFixed(2)}** e inclui remoção de tártaro, polimento e aplicação de flúor.\n\nO procedimento é rápido (cerca de 40 minutos), indolor e deixa seus dentes muito mais brancos e saudáveis! Recomendamos fazer a cada 6 meses.\n\nEsta semana estamos com preço promocional! Quer aproveitar? 😁`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
        
        // Resposta para procedimentos de harmonização
        else if (specificService === "aplicação de botox" || specificService === "preenchimento labial" || 
                 specificService === "bichectomia" || specificService === "lifting facial" || 
                 specificService === "bioestimulador de colágeno" || specificService === "harmonização facial") {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Nosso tratamento de ${specificService} é REFERÊNCIA em resultados naturais! ✨\n\nO valor é de **R$ ${price.toFixed(2)}**, com condições especiais de pagamento.\n\nRealizamos procedimentos com produtos importados da mais alta qualidade e técnicas minimamente invasivas. Nossa equipe é especializada em harmonização orofacial e certificada internacionalmente.\n\nQuer transformar sua aparência de forma segura e natural? Agende sua avaliação GRATUITA! 💫`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
        
        // Resposta normal para outros procedimentos
        else {
          return {
            id: Date.now().toString(),
            sender: 'bot',
            content: `Você vai AMAR nosso tratamento de ${specificService}! 😍 É um dos MAIS POPULARES da clínica!\n\nO valor é de **R$ ${price.toFixed(2)}** com condições especiais de pagamento.\n\nNosso procedimento é realizado com os melhores materiais do mercado e os resultados são INCRÍVEIS!\n\nQuer agendar uma avaliação? Prometo que vai valer MUITO a pena! ✨`,
            timestamp: new Date(),
            sentiment: 'neutral',
            showServicesInfo: true
          };
        }
      }
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Esses são alguns dos nossos procedimentos mais procurados! 💖\nE o melhor: todos com CONDIÇÕES ESPECIAIS de pagamento!\n\nQual deles mais chamou sua atenção? Posso te dar todos os detalhes! Temos PROMOÇÕES dessa semana que você não vai querer perder! 😉",
        timestamp: new Date(),
        sentiment: 'neutral',
        showServicesInfo: true
      };
    }
    
    // Verifica se está perguntando sobre agendamento
    if (lowerText.includes("agen") || lowerText.includes("marcar") || lowerText.includes("consulta") || 
        lowerText.includes("horário") || lowerText.includes("disponib") || lowerText.includes("atendimento") ||
        lowerText.includes("vaga") || lowerText.includes("hora") || lowerText.includes("dia")) {
      
      // Inicia o workflow de agendamento
      startWorkflow('schedulingProcess', {});
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Ótima escolha! 🌟 Nossa avaliação inicial é TOTALMENTE GRATUITA e sem compromisso!\n\nTemos horários EXCLUSIVOS ainda essa semana! E para quem agenda online, oferecemos um check-up completo com radiografia digital inclusa no pacote! 📅✨\n\nQual serviço você gostaria de agendar?",
        timestamp: new Date(),
        sentiment: 'neutral',
        workflowType: 'schedulingProcess',
        isWorkflowStep: true,
        expectsInput: true
      };
    }
    
    // Verifica se está perguntando sobre a clínica, seus diferenciais ou por que escolhê-la
    if (lowerText.includes("clínica") || lowerText.includes("lugar") || lowerText.includes("estabelecimento") || 
        lowerText.includes("diferencial") || lowerText.includes("vantagem") || lowerText.includes("por que escolher") ||
        lowerText.includes("por que vocês") || lowerText.includes("porque escolher") || lowerText.includes("porque vocês") ||
        lowerText.includes("por que devo") || lowerText.includes("porque devo") || lowerText.includes("vale a pena") ||
        lowerText.includes("profissionais") || lowerText.includes("equipe") || lowerText.includes("médicos") ||
        lowerText.includes("dentista") || lowerText.includes("doutor") || lowerText.includes("doutora")) {
      
      // Respostas alternativas do script fornecido
      const clinicResponses = [
        "Porque aqui você não é só mais um paciente, você é único para nós! 💖\nNossa missão é transformar vidas com carinho, responsabilidade e resultados incríveis! ✨\nTemos profissionais premiados, tecnologia de ponta e o atendimento mais humano que você vai encontrar! 🏆\nSeu sorriso e sua autoestima merecem o melhor... e o melhor está aqui! 😍",
        
        "Porque a gente entrega o que promete: resultados de alta qualidade sem pesar no seu bolso! 💳💥\nVocê pode parcelar tudo de forma super tranquila, com preços justos e ofertas especiais!\nTudo isso feito por profissionais experientes e apaixonados pelo que fazem!\nA sua felicidade é o que move a gente! 🚀",
        
        "Porque você merece se olhar no espelho e se sentir incrível todos os dias! 💖\nA nossa clínica é especializada em transformar autoestima, com procedimentos seguros, modernos e personalizados para você!\nAqui, a gente acredita que um sorriso bonito muda o mundo ao seu redor — e queremos construir isso junto com você! 😍",
        
        "Porque somos especialistas em entregar qualidade, segurança e atendimento humanizado! 👩‍⚕️👨‍⚕️\nTemos estrutura moderna, profissionais certificados e preços que cabem no seu bolso com facilidade no pagamento! 💳\nSe você busca ser tratado(a) com respeito, atenção e sair daqui feliz da vida, então já encontrou o lugar certo! 🎯",
        
        "Porque aqui o seu sorriso é levado a sério, mas o atendimento é leve e cheio de alegria! 😁✨\nCuidar de você é um privilégio para a nossa equipe!\nAlém disso, temos descontos exclusivos, parcelamento sem estresse e um ambiente acolhedor que vai fazer você se sentir em casa! 🏡\nVamos juntos deixar você ainda mais radiante? 🌟"
      ];
      
      // Escolhe uma resposta aleatória do array
      const randomResponse = clinicResponses[Math.floor(Math.random() * clinicResponses.length)];
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: randomResponse + "\n\nQuer conhecer nosso espaço? Agende uma visita e ganhe uma AVALIAÇÃO COMPLETA grátis! 🎉",
        timestamp: new Date(),
        sentiment: 'neutral',
        showClinicInfo: true
      };
    }
    
    // Verifica se está perguntando sobre resultados
    if (lowerText.includes("resultado") || lowerText.includes("antes e depois") || lowerText.includes("antes depois") ||
        lowerText.includes("eficaz") || lowerText.includes("funciona") || lowerText.includes("quanto tempo") ||
        lowerText.includes("duração") || lowerText.includes("duracao") || lowerText.includes("tempo de recuperação")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Nossos resultados são EXTRAORDINÁRIOS! 🌟\n\nA maioria dos pacientes percebe diferença já na PRIMEIRA sessão! E o melhor: com mínimo desconforto e rápida recuperação!\n\nTemos mais de 5.000 casos de sucesso e um índice de satisfação de 98%! Incrível, né?\n\nQuer agendar uma consulta para conhecer casos parecidos com o seu? Tenho certeza que você vai se SURPREENDER! 😍",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Se o usuário demonstrar dúvida ou confusão
    if (lowerText.includes("não entendi") || lowerText.includes("confuso") || lowerText.includes("?") || 
        lowerText.includes("como assim") || lowerText.includes("não sei") || lowerText.includes("explica")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Sem problema! Estou aqui para te ajudar com calma! 🫶\n\nVamos simplificar: temos tratamentos odontológicos (para seu sorriso perfeito!) e de harmonização facial (para realçar sua beleza natural!).\n\nQual área te interessa mais? Posso explicar detalhadamente cada procedimento, e o melhor: de um jeito SUPER fácil de entender! 😉",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Verifica se tem medo de dentista especificamente 
    if (lowerText.includes("medo de dentista") || lowerText.includes("pavor de dentista") || 
        lowerText.includes("trauma de dentista") || lowerText.includes("morro de medo")) {
      
      // Inicia o workflow de medo/ansiedade
      startWorkflow('fearAndAnxiety', {});
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Eu entendo COMPLETAMENTE! 🫂 Muitas pessoas sentem o mesmo!\n\nNossa clínica é especializada em pacientes que têm medo. Nossos profissionais são TREINADOS para criar um ambiente calmo e acolhedor. Temos até protocolos especiais de atendimento para pessoas ansiosas.\n\nAqui você define o ritmo! Podemos fazer pausas quando quiser, explicar cada detalhe antes e usar técnicas de relaxamento que realmente funcionam.\n\nTemos até a opção de sedação consciente para casos mais intensos! Que tal uma visita apenas para CONHECER o ambiente, sem nenhum procedimento? Muitos pacientes relatam que isso já ajuda a reduzir o medo! 😊",
        timestamp: new Date(),
        sentiment: 'neutral',
        workflowType: 'fearAndAnxiety',
        isWorkflowStep: true,
        expectsInput: true
      };
    }
      
    // Verifica se está preocupado com dor
    if (lowerText.includes("dor") || lowerText.includes("doi") || lowerText.includes("dolorido") || 
        lowerText.includes("doloroso") || lowerText.includes("anestesia") || lowerText.includes("medo") ||
        lowerText.includes("medroso") || lowerText.includes("medrosa")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Entendo sua preocupação com a dor! 💕 É mais comum do que você imagina.\n\nMas posso te garantir: a odontologia EVOLUIU MUITO! Nossos procedimentos utilizam anestesias potentes e indolores (aplicadas com técnicas que você mal sente a agulha).\n\nAlém disso, temos protocolos especiais para pacientes mais sensíveis - como anestesia prévia em gel, controle de respiração e até fones com música relaxante durante o procedimento.\n\nJá atendemos CENTENAS de pacientes que tinham o mesmo receio e hoje frequentam a clínica tranquilamente. Quer conhecer nossa abordagem sem compromisso? Podemos começar apenas com uma conversa! 😊",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Verifica se está preocupado com o custo/dinheiro
    if (lowerText.includes("sem grana") || lowerText.includes("sem dinheiro") || lowerText.includes("caro") || 
        lowerText.includes("preço alto") || lowerText.includes("valor alto") || lowerText.includes("não tenho como pagar") || 
        lowerText.includes("nao tenho como pagar") || lowerText.includes("fora do orçamento")) {
      
      // Inicia o workflow de preocupações financeiras
      startWorkflow('financialConcerns', {});
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Entendo sua preocupação com os valores! 💰 Mas temos ÓTIMAS NOTÍCIAS!\n\nNossa clínica tem opções para TODOS os orçamentos! Oferecemos:\n\n• Parcelamento em até 12x SEM JUROS\n• Descontos especiais para pacotes de tratamento\n• Planos mensais com valor fixo\n• Promoções sazonais (e temos uma AGORA!)\n\nMuitas pessoas se surpreendem quando descobrem que cuidar da saúde bucal pode caber no orçamento! E lembre-se: nossa avaliação inicial é TOTALMENTE GRATUITA, assim você conhece todas as opções antes de decidir.\n\nQual tratamento você está considerando realizar?",
        timestamp: new Date(),
        sentiment: 'neutral',
        workflowType: 'financialConcerns',
        isWorkflowStep: true,
        expectsInput: true
      };
    }
    
    // Verifica se tem vergonha do sorriso
    if (lowerText.includes("vergonha do sorriso") || lowerText.includes("vergonha de sorrir") || 
        lowerText.includes("vergonha dos dentes") || lowerText.includes("não gosto do meu sorriso") || 
        lowerText.includes("nao gosto do meu sorriso") || lowerText.includes("escondo meu sorriso") || 
        lowerText.includes("evito sorrir") || lowerText.includes("tô com vergonha")) {
      
      // Inicia o workflow de preocupações estéticas
      startWorkflow('aestheticConcerns', {});
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Entendo perfeitamente esse sentimento! 💖 Mas saiba que MUITAS pessoas passam por isso e conseguimos transformar essa realidade!\n\nTer vergonha do sorriso afeta não só a aparência, mas a autoestima e até mesmo oportunidades sociais e profissionais. Por isso, transformar sorrisos é uma das coisas mais GRATIFICANTES do nosso trabalho!\n\nTemos diversos tratamentos que podem fazer uma diferença INCRÍVEL em pouco tempo - desde procedimentos simples como clareamento até transformações completas.\n\nPoderia me contar um pouco mais sobre o que te incomoda no seu sorriso? Assim posso te indicar as melhores opções de tratamento.",
        timestamp: new Date(),
        sentiment: 'neutral',
        workflowType: 'aestheticConcerns',
        isWorkflowStep: true,
        expectsInput: true
      };
    }
    
    // Verifica se tentou clareamento caseiro sem sucesso
    if ((lowerText.includes("clareamento") || lowerText.includes("clarear")) && 
        (lowerText.includes("casa") || lowerText.includes("caseiro") || lowerText.includes("fiz em casa")) && 
        (lowerText.includes("não deu certo") || lowerText.includes("nao deu certo") || lowerText.includes("não funcionou") || 
         lowerText.includes("nao funcionou") || lowerText.includes("ruim") || lowerText.includes("insatisfeito"))) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "É muito comum essa experiência com clareamentos caseiros! 😓 A maioria deles realmente NÃO traz os resultados esperados.\n\nIsso acontece porque produtos de farmácia têm concentração muito baixa de agentes clareadores (por segurança) e não contam com a tecnologia de ativação que usamos na clínica.\n\nNosso clareamento profissional usa géis de alta concentração e luz especial que ACELERA o processo. O resultado é muito mais rápido, intenso e duradouro!\n\nAlém disso, fazemos tudo com acompanhamento para evitar sensibilidade. Muitos pacientes que tentaram métodos caseiros ficam IMPRESSIONADOS com a diferença do tratamento profissional! Quer conhecer? Temos promoção esta semana! ✨",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Verifica se tem medo de resultado artificial
    if ((lowerText.includes("artificial") || lowerText.includes("falso") || lowerText.includes("fake") || 
         lowerText.includes("forçado") || lowerText.includes("estranho") || lowerText.includes("exagerado")) && 
        (lowerText.includes("resultado") || lowerText.includes("aparência") || lowerText.includes("aparencia") || 
         lowerText.includes("ficar") || lowerText.includes("parecer") || lowerText.includes("medo"))) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Essa é uma preocupação SUPER válida! 💯 Entendo completamente!\n\nNossa filosofia é justamente criar resultados NATURAIS que valorizem sua beleza única, não transformações artificiais que parecem padronizadas.\n\nNossos profissionais são especialistas em harmonização e estética com abordagem conservadora. Trabalhamos com planejamento digital onde você pode VER previamente como ficará o resultado e aprovar antes de começarmos.\n\nTemos um portfólio imenso de casos onde os pacientes relatam que amigos e familiares perceberam que estão mais bonitos, mas não conseguem identificar exatamente o que mudou - esse é o sinal de um trabalho bem feito!\n\nQuer conhecer alguns desses resultados na consulta de avaliação? Você vai se surpreender! 😊",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Verifica se o usuário está querendo avançar/prosseguir/continuar
    if (lowerText.includes("prosseguir") || lowerText.includes("continuar") || lowerText.includes("avançar") || 
        lowerText.includes("seguir") || lowerText.includes("próximo") || lowerText.includes("ok") || 
        lowerText.includes("vamos lá") || lowerText.includes("próxima etapa")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Ótimo! 👍 Estou aqui para continuar te atendendo. O que mais gostaria de saber? Posso falar sobre nossos serviços, preços, horários disponíveis ou tirar qualquer dúvida! 😊",
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Verifica se o usuário está tentando encerrar a conversa
    if (lowerText.includes("tchau") || lowerText.includes("adeus") || lowerText.includes("até logo") || 
        lowerText.includes("até mais") || lowerText.includes("finalizar") || lowerText.includes("encerrar") ||
        lowerText.includes("terminar") || lowerText.includes("bye") || lowerText.includes("sair")) {
      
      // Aqui sim, podemos tentar uma última venda antes do usuário sair
      const promos = [
        "Antes de ir, que tal aproveitar nossa SUPER PROMOÇÃO de clareamento dental? 50% OFF na segunda sessão! 🤩 Só até o fim da semana!",
        "Espere! Temos uma oferta ESPECIAL hoje! Botox + preenchimento com 30% OFF! ✨ Não vai perder essa chance, vai?",
        "Antes de se despedir, saiba que estamos com as ÚLTIMAS VAGAS para avaliação GRATUITA esta semana! 📅 Posso reservar uma para você?",
        "Só um momento! Para você, temos um desconto EXCLUSIVO de primeira consulta! 💫 Quer aproveitar agora?",
        "Ei, não vá ainda! Acabamos de lançar um PACOTE VIP com preços imbatíveis! 🔝 Posso te mostrar rapidinho?"
      ];
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: `${promos[Math.floor(Math.random() * promos.length)]}\n\nMas se precisar ir, tudo bem! Estarei aqui quando voltar! 👋 Foi um prazer te atender!`,
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Mensagem de promoção (só aparecer se o usuário estiver inativo)
    if (messages.length > 3 && isUserInactive()) {
      const promos = [
        "Ainda está aí? Sabia que estamos com uma SUPER PROMOÇÃO de clareamento dental essa semana? 50% OFF na segunda sessão! Quer aproveitar? 🤩",
        "Lembrei de algo que pode te interessar! Nosso combo de harmonização facial está com desconto INCRÍVEL! Botox + preenchimento com 30% OFF! Quer saber mais? ✨",
        "Enquanto você pensa, deixa eu te contar: estamos com as últimas vagas para avaliação GRATUITA essa semana! Vamos agendar a sua? 📅",
        "Ei, você sabia que nossos PACOTES PROMOCIONAIS podem caber no seu orçamento? Posso te mostrar alguns! São oportunidades imperdíveis! 💰",
        "Aproveitando o momento: acabamos de receber os equipamentos mais modernos para tratamentos estéticos! Quer ser uma das primeiras pessoas a experimentar? 🔝"
      ];
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: `${promos[Math.floor(Math.random() * promos.length)]}\n\nFique à vontade para me perguntar o que quiser! Estou aqui exclusivamente para te ajudar! 🌎💬`,
        timestamp: new Date(),
        sentiment: 'neutral'
      };
    }
    
    // Resposta genérica para QUALQUER pergunta que não foi capturada pelas condições anteriores
    // Esta resposta é direta, sem tentar vender, apenas respondendo de forma amigável
    const defaultResponses = [
      `Entendi! 😊 Estou aqui para te ajudar com qualquer dúvida sobre nossos serviços. O que mais gostaria de saber? ${lowerText.includes("?") ? "" : "Pode me perguntar qualquer coisa!"}`,
      `Claro! 👍 Fico feliz em poder ajudar! Tem mais alguma coisa que você gostaria de saber sobre nossa clínica ou procedimentos? ${lowerText.includes("?") ? "" : "Estou à disposição!"}`,
      `Perfeito! 💯 Estou acompanhando tudo! Diga-me o que mais te interessa saber e farei o possível para ajudar! ${lowerText.includes("?") ? "" : "Estou aqui para esclarecer qualquer dúvida!"}`,
      `Anotado! 📝 Estou aqui para o que precisar! Quer que eu detalhe mais alguma coisa sobre o que conversamos? ${lowerText.includes("?") ? "" : "Posso responder qualquer pergunta sobre nossos serviços!"}`,
      `Entendido! 🌟 Estou à sua disposição para qualquer esclarecimento. Como posso continuar te ajudando? ${lowerText.includes("?") ? "" : "Não hesite em me fazer perguntas!"}`
    ];
    
    return {
      id: Date.now().toString(),
      sender: 'bot',
      content: defaultResponses[Math.floor(Math.random() * defaultResponses.length)],
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
