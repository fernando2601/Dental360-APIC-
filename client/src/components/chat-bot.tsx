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
  
  // Variável para rastrear quando foi a última interação do usuário
  const [lastInteractionTime, setLastInteractionTime] = useState<number>(Date.now());
  
  // Verifica se o usuário está inativo há mais de 5 minutos
  const isUserInactive = () => {
    return Date.now() - lastInteractionTime > 5 * 60 * 1000; // 5 minutos em milissegundos
  };
  
  // Gera respostas humanizadas com base no script e no sentimento
  const generateHumanizedResponse = (userText: string, sentiment: 'positive' | 'negative' | 'neutral'): Message => {
    const lowerText = userText.toLowerCase();
    
    // Atualiza o tempo da última interação
    setLastInteractionTime(Date.now());
    
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
          content: "Poxa, sinto muito por isso. 😔\nPara melhorar seu dia, aqui vai um presente especial 🎁:\n**CUPOM DE DESCONTO DE 10%** para qualquer procedimento hoje!\n\nUm sorriso novo pode transformar seu humor! Nossos tratamentos estéticos são rápidos e você já sai daqui se sentindo outra pessoa! Quer marcar uma avaliação? É TOTALMENTE gratuita! 💬",
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
      
      // Detectar se está perguntando sobre um serviço específico
      let specificService = "";
      
      if (lowerText.includes("limpeza")) specificService = "limpeza dental";
      else if (lowerText.includes("clareamento")) specificService = "clareamento dental";
      else if (lowerText.includes("aparelho") || lowerText.includes("ortodon")) specificService = "aparelho ortodôntico";
      else if (lowerText.includes("implante")) specificService = "implante dentário";
      else if (lowerText.includes("botox")) specificService = "aplicação de botox";
      else if (lowerText.includes("preenchimento") || lowerText.includes("labial")) specificService = "preenchimento labial";
      else if (lowerText.includes("bichectomia")) specificService = "bichectomia";
      else if (lowerText.includes("lifting") || lowerText.includes("fios")) specificService = "lifting facial";
      else if (lowerText.includes("colágeno") || lowerText.includes("colageno")) specificService = "bioestimulador de colágeno";
      
      if (specificService) {
        return {
          id: Date.now().toString(),
          sender: 'bot',
          content: `Você vai AMAR nosso tratamento de ${specificService}! 😍 É um dos MAIS POPULARES da clínica! Temos os melhores preços do mercado e resultados INCRÍVEIS!\n\nQuer agendar uma avaliação para conhecer mais detalhes? Prometo que vai valer MUITO a pena! ✨`,
          timestamp: new Date(),
          sentiment: 'neutral',
          showServicesInfo: true
        };
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
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Ótima escolha! 🌟 Nossa avaliação inicial é TOTALMENTE GRATUITA e sem compromisso!\n\nTemos horários EXCLUSIVOS ainda essa semana! E para quem agenda online, oferecemos um check-up completo com radiografia digital inclusa no pacote! 📅✨\n\nQual o melhor dia para você? Manhã ou tarde?",
        timestamp: new Date(),
        sentiment: 'neutral',
        showScheduleInfo: true
      };
    }
    
    // Verifica se está perguntando sobre a clínica
    if (lowerText.includes("clínica") || lowerText.includes("lugar") || lowerText.includes("estabelecimento") || 
        lowerText.includes("diferencial") || lowerText.includes("vantagem") || lowerText.includes("por que escolher") ||
        lowerText.includes("profissionais") || lowerText.includes("equipe") || lowerText.includes("médicos") ||
        lowerText.includes("dentista") || lowerText.includes("doutor") || lowerText.includes("doutora")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Por que escolher a nossa clínica? 😍\n\n✨ Profissionais PREMIADOS e apaixonados pelo que fazem\n✨ Atendimento VIP acolhedor e humanizado\n✨ Equipamentos ULTRA modernos para seu conforto e segurança\n✨ Resultados NATURAIS e personalizados para você!\n✨ Garantia em TODOS os tratamentos!\n\nAqui você não é só mais um paciente, você é parte da nossa família 💖\n\nQuer conhecer nosso espaço? Agende uma visita e ganhe uma AVALIAÇÃO COMPLETA grátis!",
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
    
    // Verifica se está preocupado com dor
    if (lowerText.includes("dor") || lowerText.includes("doi") || lowerText.includes("dolorido") || 
        lowerText.includes("doloroso") || lowerText.includes("anestesia") || lowerText.includes("medo")) {
      
      return {
        id: Date.now().toString(),
        sender: 'bot',
        content: "Entendo sua preocupação! 💕 Mas fique tranquilo(a)!\n\nNossos procedimentos são praticamente INDOLORES! Usamos as técnicas mais modernas e anestesias de última geração.\n\nMuitos pacientes relatam que sentem MENOS desconforto do que esperavam! E nossa equipe é ESPECIALISTA em atender pessoas com medo ou ansiedade.\n\nQuer agendar uma CONVERSA sem compromisso com nossos especialistas? Eles podem explicar tudo pessoalmente! 😊",
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
