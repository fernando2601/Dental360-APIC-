import { useState, useRef, useEffect, useCallback } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageSquare, X, Maximize2, Minimize2, Send, User, Bot, Sparkles } from "lucide-react";
import { formatTimeAgo } from "@/lib/utils";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";

// Tipagem para as mensagens
type Message = {
  id: string;
  sender: 'user' | 'bot';
  content: string;
  timestamp: Date;
};

// Interface para controle de contexto da conversa
interface ChatContext {
  lastInteraction: Date;
  hasGivenDiscount: boolean;
  discountAmount: number;
  sentimentDetected: 'positive' | 'negative' | 'neutral';
  needsFollowUp: boolean;
  followUpTime: Date | null;
  mentionedPrice: boolean;
  mentionedFamilyLoss: boolean;
  paymentMethod: string | null;
  interestedInService: string | null;
  hasSevereMentalState: boolean;
  recentTopics: string[];
  frequentQuestions: string[];
}

// Interface para sugestões de IA
interface AISuggestion {
  id: string;
  text: string;
  type: 'general' | 'appointment' | 'service' | 'payment' | 'discount';
  context?: string;
}

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
  default: "Estou aqui para te ajudar com qualquer dúvida sobre tratamentos dentários ou de harmonização! 😊\nQuer informações sobre algum procedimento específico ou prefere agendar uma avaliação gratuita?",
  inactivity: "Ainda está por aí? 😊 Estou aqui esperando suas perguntas ou podemos continuar nossa conversa depois se preferir!",
  goodbye: "Foi um prazer conversar com você! Estarei aqui quando precisar de informações ou quiser agendar sua consulta. Tenha um ótimo dia e volte sempre! 😊✨",
  // Novas respostas para objeções de vendas
  expensive: "Entendo sua preocupação com os valores! 💙\n\nMas veja bem, investir na sua saúde bucal e autoestima é um dos melhores investimentos que você pode fazer. E temos várias opções para facilitar:\n\n✅ Parcelamento em até 12x sem juros\n✅ Descontos para pacotes de tratamento\n✅ Primeira avaliação totalmente gratuita\n\nQual opção se encaixa melhor no seu orçamento? Podemos encontrar uma solução personalizada para você! 😊",
  expensive_alt: "Compreendo completamente! 💯\n\nMas sabia que oferecemos o melhor custo-benefício da região? Nossos tratamentos têm garantia e usamos materiais de altíssima qualidade que duram muito mais.\n\nAlém disso, para novos pacientes, estamos com um desconto especial de 10% no primeiro procedimento!\n\nPosso te mostrar algumas opções que cabem no seu bolso? 💸",
  expensive_extra: "Posso entender sua preocupação! 😊\n\nMas olha só: trabalhamos com planos personalizados que se adaptam à sua realidade financeira. E muitas vezes o que parece mais caro acaba sendo mais econômico a longo prazo!\n\nQue tal conversarmos sobre as diferentes opções de pagamento? Tenho certeza que encontraremos a solução ideal para você! ✨",
  compare_prices: "Entendo que você esteja pesquisando preços! É muito importante mesmo! 👍\n\nMas além do valor, considere também a qualidade e experiência dos profissionais. Nossa equipe tem especialização internacional e usamos tecnologias que muitas clínicas nem oferecem.\n\nSe você encontrou um orçamento menor, podemos analisar e tentar igualar para não perdermos você! 💕 Posso fazer uma proposta especial?",
  no_money: "Entendo esse momento! 💙\n\nJustamente por isso temos opções flexíveis de pagamento que podem caber no seu orçamento atual. Para casos como o seu, podemos oferecer um desconto especial de 15% e parcelamento estendido.\n\nE lembre-se: adiar cuidados dentários muitas vezes significa tratamentos mais caros no futuro. Que tal pelo menos fazer uma avaliação gratuita para saber suas opções?",
  no_money_alt: "Momentos financeiros apertados acontecem com todos nós! 💪\n\nPor isso mesmo temos condições especiais pensando em situações como a sua. Que tal começarmos com uma avaliação gratuita?\n\nDepois, podemos montar um plano de tratamento em fases, priorizando o mais urgente agora e deixando o resto para quando estiver mais tranquilo financeiramente. O que acha?",
  looking_elsewhere: "Entendo que você esteja avaliando outras opções, isso é muito prudente! 👏\n\nMas antes de decidir, gostaria de destacar nossos diferenciais:\n\n✨ Garantia em todos os tratamentos\n✨ Profissionais premiados internacionalmente\n✨ Tecnologia exclusiva que reduz desconforto\n✨ Atendimento humanizado e personalizado\n\nPara que você possa comparar adequadamente, que tal agendar uma avaliação gratuita sem compromisso?",
  looking_elsewhere_alt: "Comparar é sempre importante! 😊\n\nMas quero garantir que você tenha todas as informações para uma decisão justa. Muitos de nossos pacientes vieram de outras clínicas buscando a qualidade que oferecemos.\n\nO que você está buscando especificamente? Talvez eu possa mostrar como atendemos essa necessidade de forma única! ✨",
  too_far: "Entendo a preocupação com a distância! 🗺️\n\nMas muitos pacientes vêm de longe justamente pela qualidade do nosso atendimento. Um pequeno deslocamento por um tratamento excepcional vale a pena, não acha?\n\nAlém disso, concentramos seus procedimentos para minimizar o número de visitas. E para novos pacientes que vêm de longe, oferecemos 10% de desconto no primeiro tratamento! Isso ajuda?",
  thinking_about_it: "Claro, decisões importantes merecem reflexão! 💭\n\nEnquanto você pensa, posso enviar mais informações sobre o procedimento que te interessa? Ou talvez tirar alguma dúvida específica?\n\nLembre-se que a avaliação inicial é totalmente gratuita e sem compromisso. Você conhece nossa clínica, conversa com o profissional e depois decide com calma! Quando seria um bom momento para você?",
  thinking_about_it_alt: "Tomar tempo para decidir é muito sábio! ✨\n\nQueria apenas garantir que você tem todas as informações necessárias. Existe alguma dúvida que eu possa esclarecer ou alguma preocupação específica?\n\nE lembre-se: nossas vagas para avaliação gratuita são limitadas. Se quiser garantir a sua enquanto decide, posso reservar sem compromisso! 📅",
  not_priority: "Entendo que existem muitas prioridades na vida! 💫\n\nMas sabia que problemas bucais não tratados podem afetar sua saúde geral e acabar custando muito mais no futuro?\n\nQue tal pelo menos fazer a avaliação gratuita para conhecer sua situação atual? Sem compromisso, apenas para você ter clareza do que precisa ser priorizado ou não. O que acha?",
  not_priority_alt: "Respeito totalmente suas prioridades atuais! 🙌\n\nMas é interessante considerar que muitos problemas dentários são silenciosos no início e podem se tornar mais graves (e caros) com o tempo.\n\nPodemos começar com o básico - uma limpeza profissional talvez? É rápido, acessível e mantém sua saúde bucal enquanto você planeja os próximos passos. Temos horários flexíveis para encaixar na sua rotina!",
  afraid: "Medo de dentista é muito mais comum do que você imagina! 💕\n\nNossa clínica é especializada em pacientes ansiosos e com trauma. Temos técnicas específicas que tornam o atendimento muito mais tranquilo:\n\n• Anestesia indolor com aplicação de anestésico tópico antes\n• Ambiente relaxante com música e aromaterapia\n• Sedação leve para procedimentos mais complexos\n• Atendimento no seu ritmo, sem pressão\n\nQue tal conhecer nossa abordagem com uma visita sem procedimentos? Só para você se sentir confortável com o ambiente!",
  afraid_alt: "Seu medo é totalmente compreensível e respeitamos muito isso! 🫶\n\nSabia que grande parte da nossa equipe escolheu odontologia justamente por ter passado por experiências traumáticas e querer mudar essa realidade?\n\nTemos pacientes que chegaram aqui sem conseguir sequer sentar na cadeira e hoje fazem tratamentos completos relaxados. A transformação começa com pequenos passos!\n\nPosso marcar um horário especial só para você conhecer o consultório, sem qualquer procedimento? Seria o primeiro passo!"
};

// Respostas de diferencial - usadas para variar as mensagens sobre por que escolher a clínica
const CLINIC_ADVANTAGES = [
  "Porque aqui você não é só mais um paciente, você é único para nós! 💖\nNossa missão é transformar vidas com carinho, responsabilidade e resultados incríveis! ✨\nTemos profissionais premiados, tecnologia de ponta e o atendimento mais humano que você vai encontrar! 🏆\nSeu sorriso e sua autoestima merecem o melhor... e o melhor está aqui! 😍",
  
  "Porque a gente entrega o que promete: resultados de alta qualidade sem pesar no seu bolso! 💳💥\nVocê pode parcelar tudo de forma super tranquila, com preços justos e ofertas especiais!\nTudo isso feito por profissionais experientes e apaixonados pelo que fazem!\nA sua felicidade é o que move a gente! 🚀",
  
  "Porque você merece se olhar no espelho e se sentir incrível todos os dias! 💖\nA nossa clínica é especializada em transformar autoestima, com procedimentos seguros, modernos e personalizados para você!\nAqui, a gente acredita que um sorriso bonito muda o mundo ao seu redor — e queremos construir isso junto com você! 😍",
  
  "Porque somos especialistas em entregar qualidade, segurança e atendimento humanizado! 👩‍⚕️👨‍⚕️\nTemos estrutura moderna, profissionais certificados e preços que cabem no seu bolso com facilidade no pagamento! 💳\nSe você busca ser tratado(a) com respeito, atenção e sair daqui feliz da vida, então já encontrou o lugar certo! 🎯",
  
  "Porque aqui o seu sorriso é levado a sério, mas o atendimento é leve e cheio de alegria! 😁✨\nCuidar de você é um privilégio para a nossa equipe!\nAlém disso, temos descontos exclusivos, parcelamento sem estresse e um ambiente acolhedor que vai fazer você se sentir em casa! 🏡\nVamos juntos deixar você ainda mais radiante? 🌟"
];

// Sugestões de respostas inteligentes - baseadas em contexto
const AI_SUGGESTIONS: Record<string, AISuggestion[]> = {
  initial: [
    { id: 'sug_1', text: 'Quais serviços vocês oferecem?', type: 'general' },
    { id: 'sug_2', text: 'Quanto custa um clareamento dental?', type: 'service' },
    { id: 'sug_3', text: 'Quero agendar uma consulta', type: 'appointment' }
  ],
  services: [
    { id: 'sug_srv_1', text: 'Quero saber mais sobre clareamento', type: 'service' },
    { id: 'sug_srv_2', text: 'Preciso extrair o siso', type: 'service' },
    { id: 'sug_srv_3', text: 'Quanto custa o botox?', type: 'service' },
    { id: 'sug_srv_4', text: 'Como é feita a limpeza?', type: 'service' }
  ],
  pricing: [
    { id: 'sug_price_1', text: 'É possível parcelar?', type: 'payment' },
    { id: 'sug_price_2', text: 'Vocês aceitam PIX?', type: 'payment' },
    { id: 'sug_price_3', text: 'Tem desconto para pacote?', type: 'discount' },
    { id: 'sug_price_4', text: 'Está um pouco caro pra mim', type: 'discount' }
  ],
  appointment: [
    { id: 'sug_apt_1', text: 'Tem horário essa semana?', type: 'appointment' },
    { id: 'sug_apt_2', text: 'Quanto tempo dura a consulta?', type: 'appointment' },
    { id: 'sug_apt_3', text: 'Preciso levar algo?', type: 'appointment' },
    { id: 'sug_apt_4', text: 'Tem estacionamento?', type: 'general' }
  ],
  fear: [
    { id: 'sug_fear_1', text: 'Tenho muito medo de dentista', type: 'general' },
    { id: 'sug_fear_2', text: 'Dói fazer tratamento de canal?', type: 'service' },
    { id: 'sug_fear_3', text: 'Como funciona a anestesia?', type: 'service' },
    { id: 'sug_fear_4', text: 'Posso levar acompanhante?', type: 'general' }
  ],
  aesthetics: [
    { id: 'sug_aes_1', text: 'Quanto tempo dura o botox?', type: 'service' },
    { id: 'sug_aes_2', text: 'O preenchimento é dolorido?', type: 'service' },
    { id: 'sug_aes_3', text: 'Quero melhorar meu sorriso', type: 'service' },
    { id: 'sug_aes_4', text: 'Tenho manchas nos dentes', type: 'service' }
  ]
};

// Lista de perguntas frequentes
const FREQUENT_QUESTIONS = [
  "Quanto custa o clareamento dental?",
  "Como funciona o pagamento?",
  "Vocês atendem nos finais de semana?",
  "Preciso marcar horário para avaliação?",
  "Quanto tempo dura uma limpeza?",
  "Vocês têm emergência?",
  "O aparelho invisível é confortável?",
  "Posso parcelar o tratamento?",
  "Vocês aceitam convênio?",
  "Quanto tempo dura o efeito do botox?"
];

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
  "débito": "Aceitamos todos os cartões de débito! 💳\nÉ uma forma rápida e prática de pagamento. Temos também a opção do PIX e dinheiro, se preferir. O que seria mais conveniente para você?",
  "crédito": "Aceitamos todos os cartões de crédito e parcelamos em até 12x sem juros! 💳✨\nÉ uma forma de você cuidar do seu sorriso sem pesar no orçamento. Podemos agendar seu horário agora?",
  "pagamento": RESPONSES.payment,
  "pix": "Sim, aceitamos PIX! 📱 É prático, seguro e super rápido!\nEnviamos o QR code na hora do pagamento e você pode usar o banco de sua preferência. Além do PIX, aceitamos cartões e dinheiro. Como prefere pagar?",
  "dinheiro_pagamento": "Sim, aceitamos pagamento em dinheiro! 💵\nPara pagamentos à vista em dinheiro, oferecemos um desconto especial de 5%! Também aceitamos PIX e cartões se for mais conveniente. O que seria melhor para você?",
  
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
  
  // Diferencial - vamos variar as respostas
  "por que": RESPONSES.advantages,
  "vantagem": RESPONSES.advantages,
  "diferencial": RESPONSES.advantages,
  "melhor": RESPONSES.advantages,
  
  // Dúvidas e traumas
  "dor": "Fique tranquilo(a), trabalhamos com anestesia moderna e técnicas suaves! 😌\nNosso objetivo é zero desconforto durante os procedimentos. Podemos agendar uma consulta para você conhecer nossa abordagem?",
  "trauma": "Entendemos totalmente! 💕\nNossa clínica é especializada em atender pacientes com trauma de dentista. Vamos no seu ritmo, com muito carinho e paciência. Quer dar uma chance para nós?",
  
  // Agendamento
  "agendar": RESPONSES.schedule,
  "marcar": RESPONSES.schedule,
  "consulta": RESPONSES.schedule,
  "horário": RESPONSES.schedule,
  "avaliação": RESPONSES.schedule,
  
  // Objeções de vendas
  "caro": RESPONSES.expensive,
  "cara": RESPONSES.expensive,
  "preços_alt": RESPONSES.compare_prices,
  "comparando_precos": RESPONSES.compare_prices,
  "pesquisando": RESPONSES.compare_prices,
  "orçamento": RESPONSES.compare_prices,
  "dinheiro_falta": RESPONSES.no_money,
  "não tenho": RESPONSES.no_money,
  "sem grana": RESPONSES.no_money_alt,
  "apertado": RESPONSES.no_money_alt,
  "outra clínica": RESPONSES.looking_elsewhere,
  "estou vendo": RESPONSES.looking_elsewhere,
  "comparando_clinica": RESPONSES.looking_elsewhere_alt,
  "longe": RESPONSES.too_far,
  "distante": RESPONSES.too_far,
  "pensar": RESPONSES.thinking_about_it,
  "vou pensar": RESPONSES.thinking_about_it,
  "refletir": RESPONSES.thinking_about_it_alt,
  "decidir": RESPONSES.thinking_about_it_alt,
  "prioridade": RESPONSES.not_priority,
  "agora não": RESPONSES.not_priority,
  "momento": RESPONSES.not_priority_alt,
  "medo_dentista": RESPONSES.afraid,
  "receio": RESPONSES.afraid_alt,
  "pavor": RESPONSES.afraid,
  "ansiedade": RESPONSES.afraid_alt,
};

// Componente de Chatbot
export function ChatBot() {
  const [isOpen, setIsOpen] = useState(false);
  const [isMinimized, setIsMinimized] = useState(false);
  const [input, setInput] = useState("");
  const [messages, setMessages] = useState<Message[]>([]);
  const [isTyping, setIsTyping] = useState(false);
  const [inactivityTimer, setInactivityTimer] = useState<NodeJS.Timeout | null>(null);
  const [currentSuggestions, setCurrentSuggestions] = useState<AISuggestion[]>([]);
  const [suggestionsType, setSuggestionsType] = useState<string>("initial");
  const [chatContext, setChatContext] = useState<ChatContext>({
    lastInteraction: new Date(),
    hasGivenDiscount: false,
    discountAmount: 0,
    sentimentDetected: 'neutral',
    needsFollowUp: false,
    followUpTime: null,
    mentionedPrice: false,
    mentionedFamilyLoss: false,
    paymentMethod: null,
    interestedInService: null,
    hasSevereMentalState: false,
    recentTopics: [],
    frequentQuestions: []
  });
  
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
    
    // Configurar timer de inatividade inicial
    resetInactivityTimer();
    
    // Limpar o timer quando o componente for desmontado
    return () => {
      if (inactivityTimer) clearTimeout(inactivityTimer);
    };
  }, []);

  // Função para reiniciar o timer de inatividade
  const resetInactivityTimer = () => {
    // Limpar timer atual se existir
    if (inactivityTimer) clearTimeout(inactivityTimer);
    
    // Configurar novo timer de 5 minutos
    const timer = setTimeout(() => {
      handleInactivity();
    }, 5 * 60 * 1000); // 5 minutos
    
    setInactivityTimer(timer);
  };

  // Função para lidar com inatividade
  const handleInactivity = () => {
    if (messages.length > 1 && isOpen) {
      const botResponse: Message = {
        id: Date.now().toString(),
        sender: 'bot',
        content: RESPONSES.inactivity,
        timestamp: new Date()
      };
      
      setMessages(prev => [...prev, botResponse]);
      
      // Configurar outro timer para encerrar a conversa após mais 1 minuto
      setTimeout(() => {
        handleGoodbye();
      }, 1 * 60 * 1000); // 1 minuto
    }
  };

  // Função para encerrar a conversa educadamente
  const handleGoodbye = () => {
    if (isOpen) {
      const botResponse: Message = {
        id: Date.now().toString(),
        sender: 'bot',
        content: RESPONSES.goodbye,
        timestamp: new Date()
      };
      
      setMessages(prev => [...prev, botResponse]);
      
      // Opcional: fechar o chat após alguns segundos
      setTimeout(() => {
        setIsOpen(false);
      }, 10 * 1000); // 10 segundos
    }
  };

  // Rola para a mensagem mais recente e atualiza sugestões de IA
  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
    
    // Atualizar o tempo da última interação para mensagens do bot
    if (messages.length > 0 && messages[messages.length - 1].sender === 'bot') {
      setChatContext(prev => ({
        ...prev,
        lastInteraction: new Date()
      }));
      
      // Resetar o timer de inatividade
      resetInactivityTimer();
      
      // Gerar novas sugestões de IA com base no contexto atual
      updateAISuggestions();
    }
  }, [messages]);
  
  // Função para atualizar sugestões de IA baseadas no contexto da conversa
  const updateAISuggestions = useCallback(() => {
    let newSuggestionsType = "initial";
    
    // Determinar o tipo de sugestões com base no contexto
    if (chatContext.interestedInService) {
      if (chatContext.interestedInService.includes("siso") || 
          chatContext.interestedInService.includes("canal") ||
          chatContext.interestedInService.includes("implante") ||
          chatContext.interestedInService.includes("restauração")) {
        newSuggestionsType = "services";
      } else if (chatContext.interestedInService.includes("clareamento") ||
                 chatContext.interestedInService.includes("estética")) {
        newSuggestionsType = "aesthetics";
      }
    } else if (chatContext.mentionedPrice || chatContext.hasGivenDiscount) {
      newSuggestionsType = "pricing";
    } else if (messages.some(m => m.content.toLowerCase().includes("medo") || 
                            m.content.toLowerCase().includes("receio") ||
                            m.content.toLowerCase().includes("trauma"))) {
      newSuggestionsType = "fear";
    } else if (messages.some(m => m.content.toLowerCase().includes("agendar") || 
                                  m.content.toLowerCase().includes("marcar") ||
                                  m.content.toLowerCase().includes("consulta"))) {
      newSuggestionsType = "appointment";
    }
    
    // Se o tipo mudou, atualizar sugestões
    if (newSuggestionsType !== suggestionsType) {
      setSuggestionsType(newSuggestionsType);
      setCurrentSuggestions(AI_SUGGESTIONS[newSuggestionsType]);
    }
    
    // Adicionar uma sugestão personalizada com base no histórico
    if (messages.length > 2 && chatContext.recentTopics.length > 0) {
      const lastTopic = chatContext.recentTopics[chatContext.recentTopics.length - 1];
      
      // Adicionar uma sugestão personalizada baseada no tópico recente
      if (lastTopic === "clareamento") {
        const customSuggestion: AISuggestion = { 
          id: `custom_${Date.now()}`, 
          text: "Quanto tempo dura o efeito do clareamento?", 
          type: "service" 
        };
        
        if (!currentSuggestions.some(s => s.text.includes("dura o efeito"))) {
          setCurrentSuggestions(prev => [...prev.slice(0, 3), customSuggestion]);
        }
      } else if (lastTopic === "payment") {
        const customSuggestion: AISuggestion = { 
          id: `custom_${Date.now()}`, 
          text: "Tem desconto para pagamento à vista?", 
          type: "payment" 
        };
        
        if (!currentSuggestions.some(s => s.text.includes("desconto"))) {
          setCurrentSuggestions(prev => [...prev.slice(0, 3), customSuggestion]);
        }
      }
    }
  }, [chatContext, messages, suggestionsType, currentSuggestions]);

  // Função para obter uma resposta de diferencial aleatória
  const getRandomAdvantage = (): string => {
    const randomIndex = Math.floor(Math.random() * CLINIC_ADVANTAGES.length);
    return CLINIC_ADVANTAGES[randomIndex];
  };

  // Função para gerar resposta baseada em palavras-chave e contexto da conversa
  const generateResponse = (message: string): string => {
    const lowerMessage = message.toLowerCase();
    let newContext = { ...chatContext };
    
    // Atualizar contexto com a mensagem atual
    newContext.lastInteraction = new Date();
    
    // Verifica perda familiar - oferece desconto de 15%
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
      newContext.mentionedFamilyLoss = true;
      newContext.hasGivenDiscount = true;
      newContext.discountAmount = 15;
      setChatContext(newContext);
      return "Sinto muito pela sua perda! 💔 Momentos difíceis como esse nos lembram de cuidar de nós mesmos. Para ajudar nesse momento, queremos oferecer um cupom especial de 15% de desconto em qualquer tratamento. Podemos te ajudar de alguma forma?";
    }
    
    // Detecção de estado mental severo - oferece desconto de 20% (o máximo)
    if ((lowerMessage.includes("depressão") || 
         lowerMessage.includes("depressivo") || 
         lowerMessage.includes("suicídio") || 
         lowerMessage.includes("suicida") || 
         lowerMessage.includes("muito mal") ||
         lowerMessage.includes("terrível") ||
         lowerMessage.includes("horrível") ||
         lowerMessage.includes("desesperado")) && 
        (lowerMessage.includes("sinto") || 
         lowerMessage.includes("estou") || 
         lowerMessage.includes("me sinto"))) {
      newContext.hasSevereMentalState = true;
      newContext.hasGivenDiscount = true;
      newContext.discountAmount = 20;
      setChatContext(newContext);
      return "Sinto muito que você esteja passando por esse momento tão difícil. 💙 Sua saúde mental é muito importante, e recomendo buscar apoio profissional especializado.\n\nPara apoiar você nesse momento desafiador, gostaríamos de oferecer um desconto especial de 20% em qualquer procedimento, e vamos priorizar seu bem-estar em todo o processo. Estamos aqui para ajudar. Posso agendar um horário especial para você?";
    }
    
    // Verifica objeções de preço/valor quando menciona "caro" ou similares
    if (lowerMessage.includes("caro") || 
        lowerMessage.includes("cara") || 
        lowerMessage.includes("muito caro") || 
        lowerMessage.includes("preço alto") || 
        lowerMessage.includes("valor alto") ||
        lowerMessage.includes("não tenho dinheiro") ||
        lowerMessage.includes("sem grana")) {
      
      newContext.mentionedPrice = true;
      
      // Se já ofereceu desconto, usa outra abordagem
      if (chatContext.hasGivenDiscount) {
        setChatContext(newContext);
        return Math.random() > 0.5 ? RESPONSES.expensive_alt : RESPONSES.expensive_extra;
      }
      
      // Se ainda não ofereceu desconto, oferece 10%
      newContext.hasGivenDiscount = true;
      newContext.discountAmount = 10;
      setChatContext(newContext);
      return RESPONSES.expensive;
    }
    
    // Verifica se está comparando preços ou pesquisando outras clínicas
    if (lowerMessage.includes("comparando preços") || 
        lowerMessage.includes("pesquisando") || 
        lowerMessage.includes("outras clínicas") ||
        lowerMessage.includes("outro lugar") ||
        lowerMessage.includes("vou procurar") ||
        lowerMessage.includes("mais barato")) {
      
      // Se ainda não ofereceu desconto, oferece 10%
      if (!chatContext.hasGivenDiscount) {
        newContext.hasGivenDiscount = true;
        newContext.discountAmount = 10;
        setChatContext(newContext);
        return "Entendo que você esteja comparando opções! 👍\n\nPara facilitar sua decisão, posso oferecer um desconto especial de 10% para você fechar conosco hoje. Além disso, temos parcelamento em até 12x sem juros.\n\nNossa clínica é reconhecida pela qualidade e resultados duradouros. Isso acaba sendo mais econômico a longo prazo! Posso agendar sua avaliação gratuita?";
      }
      
      // Se já ofereceu desconto, usa uma outra abordagem de convencimento
      setChatContext(newContext);
      return Math.random() > 0.5 ? RESPONSES.looking_elsewhere : RESPONSES.looking_elsewhere_alt;
    }
    
    // Detecta menções a métodos de pagamento específicos
    if (lowerMessage.includes("pix")) {
      newContext.paymentMethod = "pix";
      setChatContext(newContext);
      return "O PIX é uma ótima escolha! 📱 É rápido, prático e seguro.\n\nGeramos o QR code na hora do pagamento, e você pode utilizar o banco de sua preferência. Para pagamentos via PIX, oferecemos um desconto adicional de 3%!\n\nQual procedimento você está interessado(a)? Assim posso passar valores mais precisos.";
    } else if (lowerMessage.includes("débito")) {
      newContext.paymentMethod = "débito";
      setChatContext(newContext);
      return "Aceitamos todos os cartões de débito! 💳\n\nÉ uma forma prática e segura de pagamento. O valor é debitado na hora da sua conta, sem complicações.\n\nTemos máquinas de todas as bandeiras principais. Há algum procedimento específico que você gostaria de saber o valor?";
    } else if (lowerMessage.includes("crédito")) {
      newContext.paymentMethod = "crédito";
      setChatContext(newContext);
      return "Parcelamos em até 12x sem juros no cartão de crédito! 💳✨\n\nIsso permite que você comece seu tratamento imediatamente sem pesar no orçamento mensal. Aceitamos todas as bandeiras principais.\n\nQual procedimento você tem interesse? Posso calcular as parcelas para você visualizar melhor!";
    }
    
    // Detecção de sentimentos persistentes de tristeza e oferecimento de desconto
    if ((lowerMessage === "triste" || 
         lowerMessage === "mal" || 
         lowerMessage === "péssimo" || 
         lowerMessage === "péssima" || 
         lowerMessage === "ruim" ||
         lowerMessage === "cansado" ||
         lowerMessage === "cansada" ||
         lowerMessage === "desanimado" ||
         lowerMessage === "desanimada") && 
        chatContext.sentimentDetected === 'negative') {
      
      // Se a pessoa insiste em apenas dizer palavras negativas, aumenta o desconto
      if (!chatContext.hasGivenDiscount) {
        newContext.hasGivenDiscount = true;
        newContext.discountAmount = 15;
        setChatContext(newContext);
        return "Percebo que você não está em um bom momento, e isso me preocupa. 💙\n\nQuero te oferecer algo especial: um desconto de 15% em qualquer tratamento que escolher.\n\nÀs vezes, transformar o sorriso é o primeiro passo para se sentir melhor! Podemos ajudar você nessa jornada? Que tal uma consulta para conhecer nossas opções?";
      } else if (chatContext.discountAmount < 20) {
        // Se já ofereceu desconto, mas a pessoa continua com problemas, aumenta para o máximo de 20%
        newContext.discountAmount = 20;
        setChatContext(newContext);
        return "Sinto muito que você continue se sentindo assim. 😔💕\n\nQuero fazer algo especial por você: vou aumentar seu desconto para 20% (nosso máximo!) em qualquer tratamento.\n\nAlém disso, nossos profissionais são conhecidos pelo atendimento acolhedor e humano. Podemos reservar um horário especial para você, com mais tempo e atenção. O que acha?";
      }
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
        // Identifica o serviço específico
        if (phrase.includes("juízo") || phrase.includes("siso")) {
          newContext.interestedInService = "extração de siso";
        } else if (phrase.includes("branquinho") || phrase.includes("branco")) {
          newContext.interestedInService = "clareamento";
        } else if (phrase.includes("quebrado")) {
          newContext.interestedInService = "restauração";
        } else if (phrase.includes("podre")) {
          newContext.interestedInService = "tratamento dental";
        } else if (phrase.includes("consertar") || phrase.includes("sorriso")) {
          newContext.interestedInService = "estética dental";
        }
        
        setChatContext(newContext);
        
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
    
    // Checa por palavras-chave de sentimento
    if (lowerMessage.includes("bem") || 
        lowerMessage.includes("feliz") || 
        lowerMessage.includes("ótimo") || 
        lowerMessage.includes("ótima")) {
      newContext.sentimentDetected = 'positive';
      setChatContext(newContext);
      return RESPONSES.positive;
    } else if (lowerMessage.includes("mal") || 
               lowerMessage.includes("triste") || 
               lowerMessage.includes("péssimo") || 
               lowerMessage.includes("péssima") || 
               lowerMessage.includes("ruim") ||
               lowerMessage.includes("cansado") ||
               lowerMessage.includes("cansada")) {
      newContext.sentimentDetected = 'negative';
      setChatContext(newContext);
      
      // Se ainda não ofereceu desconto, oferece 15%
      if (!chatContext.hasGivenDiscount) {
        newContext.hasGivenDiscount = true;
        newContext.discountAmount = 15;
        setChatContext(newContext);
      }
      
      return RESPONSES.negative;
    }
    
    // Checa por perguntas sobre diferencial da clínica - responde com variações
    if (lowerMessage.includes("por que") || 
        lowerMessage.includes("vantagem") || 
        lowerMessage.includes("diferencial") || 
        lowerMessage.includes("melhor") ||
        lowerMessage.includes("escolher vocês")) {
      return getRandomAdvantage();
    }
    
    // Checa por palavras-chave individuais de serviços e objeções
    for (const [keyword, response] of Object.entries(KEYWORDS)) {
      if (lowerMessage.includes(keyword)) {
        // Se for uma palavra-chave de serviço, atualiza o serviço de interesse
        if (["siso", "juízo", "clareamento", "branqueamento", "canal", "limpeza", "aparelho", 
             "bruxismo", "ranger", "cárie", "implante", "extração", "botox", "preenchimento", 
             "facial", "bichectomia", "papada"].includes(keyword)) {
          newContext.interestedInService = keyword;
          setChatContext(newContext);
        }
        
        return response;
      }
    }
    
    // Se nenhuma palavra-chave específica foi encontrada
    return RESPONSES.default;
  };

  // Função para usar uma sugestão de IA como input
  const handleUseSuggestion = (suggestion: AISuggestion) => {
    if (isTyping) return;
    
    // Registra o tópico na lista de tópicos recentes
    setChatContext(prev => ({
      ...prev,
      recentTopics: [...prev.recentTopics, suggestion.type],
      lastInteraction: new Date()
    }));
    
    // Usa a sugestão como mensagem do usuário
    const userMessage: Message = {
      id: Date.now().toString(),
      sender: 'user',
      content: suggestion.text,
      timestamp: new Date()
    };
    
    setMessages(prev => [...prev, userMessage]);
    
    // Reinicia o timer de inatividade
    resetInactivityTimer();
    
    // Simula o chatbot digitando
    setIsTyping(true);
    
    // Determina o tempo de digitação baseado no tamanho da resposta
    const response = generateResponse(suggestion.text);
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
    
    // Detecta possíveis tópicos para adicionar ao contexto
    const lowerInput = input.toLowerCase();
    let detectedTopic = null;
    
    if (lowerInput.includes("clareamento") || lowerInput.includes("branqueamento")) {
      detectedTopic = "clareamento";
    } else if (lowerInput.includes("siso") || lowerInput.includes("juízo")) {
      detectedTopic = "extração";
    } else if (lowerInput.includes("botox") || lowerInput.includes("preenchimento")) {
      detectedTopic = "harmonização";
    } else if (lowerInput.includes("cartão") || lowerInput.includes("pix") || lowerInput.includes("pagamento")) {
      detectedTopic = "payment";
    }
    
    // Atualiza o contexto do chat
    setChatContext(prev => ({
      ...prev,
      lastInteraction: new Date(),
      recentTopics: detectedTopic ? [...prev.recentTopics, detectedTopic] : prev.recentTopics
    }));
    
    // Reinicia o timer de inatividade
    resetInactivityTimer();
    
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
      
      // Reinicia o timer de inatividade ao abrir o chat
      resetInactivityTimer();
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
              {/* AI Suggestions */}
              <div className="p-2 border-t">
                <div className="flex items-center gap-2 mb-2">
                  <Sparkles className="h-3 w-3 text-primary" />
                  <span className="text-xs font-medium text-muted-foreground">Sugestões de IA</span>
                </div>
                <div className="flex flex-wrap gap-2 mb-3">
                  {currentSuggestions.map((suggestion) => (
                    <Badge
                      key={suggestion.id}
                      variant="outline"
                      className="cursor-pointer hover:bg-accent hover:text-accent-foreground transition-colors"
                      onClick={() => handleUseSuggestion(suggestion)}
                    >
                      {suggestion.text}
                    </Badge>
                  ))}
                </div>
              </div>
              
              <CardFooter className="border-t p-2">
                <div className="flex w-full items-center gap-2">
                  <Input
                    placeholder="Digite sua mensagem..."
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && !isTyping && handleSendMessage()}
                    className="flex-1 text-sm"
                  />
                  <Popover>
                    <PopoverTrigger asChild>
                      <Button
                        variant="ghost"
                        size="icon"
                        className="shrink-0"
                        disabled={isTyping}
                      >
                        <Sparkles className="h-4 w-4" />
                      </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-80 p-0" side="top" align="end">
                      <div className="p-2 border-b">
                        <h3 className="text-sm font-medium">Perguntas frequentes</h3>
                      </div>
                      <div className="p-2 flex flex-col gap-1 max-h-60 overflow-y-auto">
                        {FREQUENT_QUESTIONS.map((question, index) => (
                          <Button
                            key={index}
                            variant="ghost"
                            className="w-full justify-start text-sm font-normal"
                            onClick={() => {
                              setInput(question);
                            }}
                          >
                            {question}
                          </Button>
                        ))}
                      </div>
                    </PopoverContent>
                  </Popover>
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