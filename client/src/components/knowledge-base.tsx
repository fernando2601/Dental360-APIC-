import React, { useState } from 'react';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Search, FileText, Download, BookOpen, FileCheck, Info, Bookmark, Star, FilePlus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from '@/components/ui/accordion';

// Tipo para materiais
interface Material {
  id: string;
  title: string;
  description: string;
  category: string;
  tags: string[];
  dateAdded: string;
  type: 'pdf' | 'video' | 'image' | 'text';
  content?: string;
  url?: string;
}

// Tipo para protocolos
interface Protocol {
  id: string;
  title: string;
  description: string;
  category: string;
  steps: {
    step: number;
    instruction: string;
    notes?: string;
  }[];
  materials: string[];
  lastUpdated: string;
}

// Tipo para formulários
interface Form {
  id: string;
  title: string;
  description: string;
  category: string;
  content: string;
  lastUpdated: string;
  required: boolean;
}

const KnowledgeBase = () => {
  const [activeTab, setActiveTab] = useState('materials');
  const [searchQuery, setSearchQuery] = useState('');

  // Dados de exemplo para materiais educativos
  const educationalMaterials: Material[] = [
    {
      id: '1',
      title: 'Cuidados pós-operatórios para extração de siso',
      description: 'Guia completo de cuidados que o paciente deve ter após a extração de dentes do siso',
      category: 'Cirúrgico',
      tags: ['pós-operatório', 'extração', 'siso'],
      dateAdded: '2025-03-15',
      type: 'pdf',
      url: '/materials/cuidados-pos-extracao.pdf',
    },
    {
      id: '2',
      title: 'Como escovar os dentes corretamente',
      description: 'Tutorial passo a passo sobre a técnica correta de escovação para adultos',
      category: 'Higiene',
      tags: ['escovação', 'higiene', 'limpeza'],
      dateAdded: '2025-02-10',
      type: 'video',
      url: 'https://www.youtube.com/watch?v=example',
    },
    {
      id: '3',
      title: 'Benefícios do clareamento dental',
      description: 'Explicação sobre os benefícios, duração e cuidados com o clareamento dental profissional',
      category: 'Estética',
      tags: ['clareamento', 'estética', 'branqueamento'],
      dateAdded: '2025-03-05',
      type: 'text',
      content: 'O clareamento dental é um procedimento estético que visa remover manchas e pigmentações dos dentes, resultando em um sorriso mais branco e brilhante. Existem diversas técnicas disponíveis, incluindo o clareamento de consultório e o clareamento caseiro supervisionado. O clareamento de consultório utiliza géis com concentrações mais altas de agentes clareadores e pode apresentar resultados imediatos. Já o clareamento caseiro utiliza moldeiras personalizadas e géis de menor concentração, devendo ser usado conforme orientação profissional. É importante ressaltar que o clareamento não funciona em restaurações, coroas ou facetas, e que a duração dos resultados varia de acordo com os hábitos do paciente, como consumo de café, vinho tinto, chá e tabaco.',
    },
    {
      id: '4',
      title: 'O que esperar em um tratamento de canal',
      description: 'Informações sobre o processo, duração e sensações durante um tratamento endodôntico',
      category: 'Endodontia',
      tags: ['canal', 'endodontia', 'tratamento'],
      dateAdded: '2025-01-20',
      type: 'pdf',
      url: '/materials/tratamento-canal.pdf',
    },
    {
      id: '5',
      title: 'Guia de Harmonização Facial',
      description: 'Explicação dos principais procedimentos de harmonização facial e seus resultados',
      category: 'Estética',
      tags: ['harmonização', 'botox', 'preenchimento'],
      dateAdded: '2025-04-02',
      type: 'image',
      url: '/materials/guia-harmonizacao.jpg',
    },
    {
      id: '6',
      title: 'Cuidados com aparelho ortodôntico',
      description: 'Instruções detalhadas sobre como limpar e cuidar do aparelho fixo',
      category: 'Ortodontia',
      tags: ['aparelho', 'ortodontia', 'limpeza'],
      dateAdded: '2025-02-28',
      type: 'pdf',
      url: '/materials/cuidados-aparelho.pdf',
    },
  ];

  // Dados de exemplo para protocolos clínicos
  const clinicalProtocols: Protocol[] = [
    {
      id: '1',
      title: 'Protocolo de atendimento para clareamento a laser',
      description: 'Passos detalhados para realização de clareamento dental a laser em consultório',
      category: 'Estética',
      steps: [
        { step: 1, instruction: 'Avaliação inicial e anamnese', notes: 'Verificar contraindicações' },
        { step: 2, instruction: 'Registro fotográfico e de cor inicial' },
        { step: 3, instruction: 'Profilaxia com pasta sem flúor' },
        { step: 4, instruction: 'Instalação do afastador labial' },
        { step: 5, instruction: 'Proteção dos tecidos gengivais com barreira fotopolimerizável' },
        { step: 6, instruction: 'Aplicação do gel clareador (peróxido de hidrogênio 35%)', notes: 'Trocar a cada 15 minutos, 3 aplicações' },
        { step: 7, instruction: 'Aplicação do laser conforme instruções do fabricante' },
        { step: 8, instruction: 'Remoção do gel e da barreira gengival' },
        { step: 9, instruction: 'Aplicação de agente dessensibilizante', notes: '5 minutos de ação' },
        { step: 10, instruction: 'Registro fotográfico final e orientações ao paciente' },
      ],
      materials: ['Gel clareador 35%', 'Barreira gengival', 'Afastador labial', 'Equipamento laser', 'Agente dessensibilizante'],
      lastUpdated: '2025-03-10',
    },
    {
      id: '2',
      title: 'Protocolo de aplicação de toxina botulínica para rugas de expressão',
      description: 'Procedimento passo a passo para aplicação de botox na região frontal e perioculares',
      category: 'Harmonização',
      steps: [
        { step: 1, instruction: 'Anamnese e avaliação da musculatura facial', notes: 'Confirmar ausência de contraindicações' },
        { step: 2, instruction: 'Registro fotográfico pré-procedimento' },
        { step: 3, instruction: 'Higienização da face com clorexidina' },
        { step: 4, instruction: 'Marcação dos pontos de aplicação', notes: 'Usar marcador estéril' },
        { step: 5, instruction: 'Reconstituição da toxina conforme instruções do fabricante' },
        { step: 6, instruction: 'Aplicação da toxina nos pontos marcados', notes: 'Região frontal: 2-4 U/ponto; Regiões perioculares: 1-2 U/ponto' },
        { step: 7, instruction: 'Orientações pós-procedimento ao paciente' },
        { step: 8, instruction: 'Agendamento para retorno em 15 dias para avaliação' },
      ],
      materials: ['Toxina botulínica', 'Seringas de insulina', 'Agulhas 30G', 'Clorexidina', 'Marcador estéril', 'Gelo'],
      lastUpdated: '2025-02-20',
    },
    {
      id: '3',
      title: 'Protocolo de bichectomia',
      description: 'Passos cirúrgicos para remoção da bola de Bichat',
      category: 'Harmonização',
      steps: [
        { step: 1, instruction: 'Anamnese e avaliação pré-cirúrgica', notes: 'Verificar condições de saúde' },
        { step: 2, instruction: 'Registro fotográfico pré-operatório' },
        { step: 3, instruction: 'Antissepsia intra e extraoral', notes: 'Clorexidina 0,12% e 2%' },
        { step: 4, instruction: 'Anestesia infiltrativa local', notes: 'Nervos bucal, infraorbitário e alveolar superior posterior' },
        { step: 5, instruction: 'Incisão da mucosa bucal', notes: 'Acima do ducto de Stenon' },
        { step: 6, instruction: 'Divulsão dos tecidos e identificação da bola de Bichat' },
        { step: 7, instruction: 'Hemostasia', notes: 'Usar pinça hemostática' },
        { step: 8, instruction: 'Remoção parcial da bola de Bichat', notes: 'Preservar parte posterior' },
        { step: 9, instruction: 'Sutura', notes: 'Fio reabsorvível 4-0' },
        { step: 10, instruction: 'Orientações pós-operatórias' },
      ],
      materials: ['Kit cirúrgico estéril', 'Anestésico com vasoconstritor', 'Pinça hemostática', 'Fio de sutura 4-0', 'Clorexidina'],
      lastUpdated: '2025-01-15',
    },
    {
      id: '4',
      title: 'Protocolo para facetas de resina composta',
      description: 'Técnica de estratificação para restaurações estéticas anteriores',
      category: 'Dentística',
      steps: [
        { step: 1, instruction: 'Seleção de cor', notes: 'Usar escala de cores com dente hidratado' },
        { step: 2, instruction: 'Isolamento absoluto do campo operatório' },
        { step: 3, instruction: 'Preparo minimamente invasivo', notes: 'Preservar esmalte quando possível' },
        { step: 4, instruction: 'Condicionamento ácido seletivo em esmalte', notes: '30 segundos' },
        { step: 5, instruction: 'Aplicação do sistema adesivo', notes: 'Seguir instruções do fabricante' },
        { step: 6, instruction: 'Confecção de guia de silicone para parede palatina' },
        { step: 7, instruction: 'Estratificação da resina', notes: 'Dentina, esmalte e efeitos' },
        { step: 8, instruction: 'Fotopolimerização por camadas' },
        { step: 9, instruction: 'Acabamento e polimento', notes: 'Usar sequência de discos e borrachas' },
        { step: 10, instruction: 'Ajuste oclusal' },
      ],
      materials: ['Resinas compostas', 'Ácido fosfórico 37%', 'Sistema adesivo', 'Silicone de adição', 'Kit de acabamento e polimento'],
      lastUpdated: '2025-04-05',
    },
    {
      id: '5',
      title: 'Protocolo de restauração com resina Bulk Fill',
      description: 'Passos para restaurações posteriores com técnica de incremento único',
      category: 'Dentística',
      steps: [
        { step: 1, instruction: 'Anestesia e isolamento absoluto' },
        { step: 2, instruction: 'Remoção de tecido cariado' },
        { step: 3, instruction: 'Proteção do complexo dentino-pulpar se necessário' },
        { step: 4, instruction: 'Condicionamento ácido total', notes: '15s em dentina, 30s em esmalte' },
        { step: 5, instruction: 'Aplicação do sistema adesivo', notes: 'Duas camadas' },
        { step: 6, instruction: 'Inserção da resina Bulk Fill em incremento único', notes: 'Até 4-5mm' },
        { step: 7, instruction: 'Fotopolimerização', notes: '20 segundos por face' },
        { step: 8, instruction: 'Camada de resina convencional na superfície oclusal', notes: '2mm' },
        { step: 9, instruction: 'Escultura oclusal' },
        { step: 10, instruction: 'Acabamento e polimento' },
      ],
      materials: ['Resina Bulk Fill', 'Resina convencional para oclusal', 'Ácido fosfórico 37%', 'Sistema adesivo', 'Material para proteção pulpar'],
      lastUpdated: '2025-03-28',
    },
  ];

  // Dados de exemplo para formulários de consentimento
  const consentForms: Form[] = [
    {
      id: '1',
      title: 'Termo de consentimento para clareamento dental',
      description: 'Documento explicando riscos, benefícios e alternativas do procedimento de clareamento',
      category: 'Estética',
      content: `TERMO DE CONSENTIMENTO LIVRE E ESCLARECIDO PARA CLAREAMENTO DENTAL

Eu, [NOME DO PACIENTE], declaro que fui informado(a) pelo(a) Dr(a). [NOME DO PROFISSIONAL] sobre o tratamento de clareamento dental, seus riscos, benefícios e alternativas.

PROCEDIMENTO: O clareamento dental é um procedimento estético que visa clarear os dentes através da aplicação de géis à base de peróxido de hidrogênio ou peróxido de carbamida. O procedimento pode ser realizado no consultório (clareamento de consultório) ou em casa com supervisão profissional (clareamento caseiro).

RESULTADOS: Estou ciente de que os resultados variam de pessoa para pessoa, dependendo da causa da alteração de cor dos dentes, e que não há garantia de resultado específico. Entendo que restaurações, coroas e facetas não clareiam com o tratamento.

RISCOS E DESCONFORTOS: Fui informado(a) sobre os possíveis efeitos colaterais, que podem incluir:
- Sensibilidade dental temporária
- Irritação gengival transitória
- Desconforto durante ou após o procedimento
- Possibilidade de recidiva da cor com o tempo

ALTERNATIVAS: Fui informado(a) sobre outras alternativas para melhorar a estética dental, como facetas, coroas ou lentes de contato dentais, e optei pelo clareamento dental.

RESPONSABILIDADES DO PACIENTE: Comprometo-me a seguir todas as orientações profissionais, incluindo:
- Comparecer a todas as consultas agendadas
- Seguir as instruções de uso do produto (no caso de clareamento caseiro)
- Evitar alimentos e bebidas com corantes nos primeiros dias após o procedimento
- Informar qualquer desconforto ou reação adversa imediatamente

AUTORIZAÇÃO: Declaro que li e entendi todas as informações contidas neste termo e que todas as minhas dúvidas foram esclarecidas. Portanto, dou meu consentimento para a realização do procedimento de clareamento dental.

[LOCAL E DATA]

_______________________
Assinatura do Paciente

_______________________
Assinatura do Profissional`,
      lastUpdated: '2025-02-10',
      required: true,
    },
    {
      id: '2',
      title: 'Consentimento informado para aplicação de toxina botulínica',
      description: 'Documento de autorização para procedimentos com botox facial',
      category: 'Harmonização',
      content: `TERMO DE CONSENTIMENTO INFORMADO PARA APLICAÇÃO DE TOXINA BOTULÍNICA

Eu, [NOME DO PACIENTE], declaro que fui devidamente informado(a) pelo(a) Dr(a). [NOME DO PROFISSIONAL] sobre o procedimento de aplicação de toxina botulínica, seus riscos, benefícios e alternativas.

PROCEDIMENTO: A toxina botulínica é uma substância que, quando injetada em pequenas doses nos músculos faciais, promove relaxamento muscular temporário, reduzindo rugas dinâmicas e linhas de expressão.

RESULTADOS ESPERADOS: Estou ciente de que os resultados são temporários (3-6 meses) e que podem ser necessárias aplicações adicionais para manutenção dos efeitos. Entendo que o efeito completo pode levar até 15 dias para se manifestar.

RISCOS E COMPLICAÇÕES POSSÍVEIS: Fui informado(a) sobre os possíveis efeitos colaterais e complicações, que podem incluir:
- Equimoses (roxos) e edemas (inchaços) transitórios no local da aplicação
- Assimetria facial temporária
- Dor ou desconforto no local da aplicação
- Ptose palpebral (queda da pálpebra) temporária
- Dor de cabeça temporária
- Reações alérgicas (raras)

CONTRAINDICAÇÕES: Declaro que informei ao profissional todas as minhas condições de saúde e que não apresento nenhuma das seguintes contraindicações:
- Gravidez ou amamentação
- Doenças neuromusculares (como Miastenia Gravis)
- Infecção ativa no local da aplicação
- Alergia conhecida à toxina botulínica ou componentes da fórmula

CUIDADOS PÓS-PROCEDIMENTO: Comprometo-me a seguir todas as recomendações pós-procedimento, incluindo:
- Não manipular a área tratada nas primeiras 4 horas
- Não realizar atividade física intensa nas primeiras 24 horas
- Evitar exposição a calor excessivo e sauna nas primeiras 48 horas
- Não deitar ou abaixar a cabeça nas primeiras 4 horas

AUTORIZAÇÃO: Declaro que li e entendi todas as informações contidas neste termo e que todas as minhas dúvidas foram esclarecidas. Autorizo o registro fotográfico para documentação científica e avaliação de resultados.

Portanto, dou meu consentimento livre e esclarecido para a realização do procedimento de aplicação de toxina botulínica.

[LOCAL E DATA]

_______________________
Assinatura do Paciente

_______________________
Assinatura do Profissional`,
      lastUpdated: '2025-03-05',
      required: true,
    },
    {
      id: '3',
      title: 'Termo de consentimento para tratamento ortodôntico',
      description: 'Documento de ciência sobre o tratamento com aparelho fixo',
      category: 'Ortodontia',
      content: `TERMO DE CONSENTIMENTO LIVRE E ESCLARECIDO PARA TRATAMENTO ORTODÔNTICO

Eu, [NOME DO PACIENTE], declaro que fui informado(a) pelo(a) Dr(a). [NOME DO PROFISSIONAL] sobre o tratamento ortodôntico proposto, seus riscos, benefícios e alternativas.

PROCEDIMENTO: O tratamento ortodôntico visa corrigir o posicionamento dos dentes e a relação entre as arcadas dentárias através do uso de aparelhos ortodônticos fixos ou removíveis, que aplicam forças controladas para movimentar os dentes.

DURAÇÃO DO TRATAMENTO: Fui informado(a) que a duração estimada do tratamento é de [DURAÇÃO ESTIMADA], podendo variar de acordo com a complexidade do caso, resposta biológica individual e colaboração com o uso dos dispositivos prescritos.

RESULTADOS ESPERADOS: Entendo que o objetivo do tratamento é melhorar a função mastigatória, a saúde bucal e a estética do sorriso, mas que existem limitações biológicas e anatômicas que podem afetar o resultado final.

DESCONFORTOS E RISCOS: Fui informado(a) sobre os possíveis desconfortos e riscos associados ao tratamento, que podem incluir:
- Desconforto nos dias seguintes às ativações
- Inflamação gengival e dificuldade de higienização
- Reabsorção radicular (encurtamento das raízes dos dentes)
- Descalcificação do esmalte (manchas brancas) por higiene deficiente
- Recidiva (retorno parcial à posição original) após o tratamento
- Disfunção temporomandibular temporária

RESPONSABILIDADES DO PACIENTE: Comprometo-me a:
- Comparecer pontualmente às consultas agendadas
- Seguir rigorosamente as orientações de higiene oral
- Usar corretamente os elásticos e aparelhos removíveis quando indicados
- Evitar alimentos duros, pegajosos ou que possam danificar o aparelho
- Usar a contenção conforme orientação após a remoção do aparelho

ALTERNATIVAS: Fui informado(a) sobre as alternativas de tratamento disponíveis para o meu caso, incluindo [ALTERNATIVAS ESPECÍFICAS].

CONTENÇÃO: Estou ciente de que após a remoção do aparelho fixo, será necessário o uso de contenção (fixa ou removível) conforme orientação profissional, para evitar a recidiva do tratamento.

AUTORIZAÇÃO: Declaro que li e entendi todas as informações contidas neste termo e que todas as minhas dúvidas foram esclarecidas. Autorizo o registro fotográfico, radiográfico e de modelos para documentação e planejamento do tratamento.

Portanto, dou meu consentimento livre e esclarecido para a realização do tratamento ortodôntico conforme plano apresentado.

[LOCAL E DATA]

_______________________
Assinatura do Paciente ou Responsável Legal

_______________________
Assinatura do Profissional`,
      lastUpdated: '2025-01-20',
      required: true,
    },
    {
      id: '4',
      title: 'Consentimento para extração de dentes do siso',
      description: 'Formulário detalhando riscos e benefícios da cirurgia dos terceiros molares',
      category: 'Cirurgia',
      content: `TERMO DE CONSENTIMENTO LIVRE E ESCLARECIDO PARA EXTRAÇÃO DE TERCEIROS MOLARES (SISOS)

Eu, [NOME DO PACIENTE], declaro que fui informado(a) pelo(a) Dr(a). [NOME DO PROFISSIONAL] sobre a necessidade de extração dos meus dentes terceiros molares (sisos), bem como sobre os riscos, benefícios e alternativas deste procedimento.

DIAGNÓSTICO E INDICAÇÃO: Com base no exame clínico e radiográfico, foi diagnosticada a necessidade de extração dos seguintes dentes terceiros molares: [DENTES A SEREM EXTRAÍDOS], devido a [RAZÕES ESPECÍFICAS: impactação, má posição, risco de cárie, pericoronarite, etc].

PROCEDIMENTO: A cirurgia será realizada sob anestesia local, podendo ser necessária sedação complementar. O procedimento envolve incisão gengival, remoção de tecido ósseo quando necessário, extração do dente, limpeza do local e sutura.

BENEFÍCIOS ESPERADOS: A remoção dos dentes do siso visa prevenir ou tratar problemas como dor, infecção, danos aos dentes adjacentes, cistos e tumores associados a dentes retidos.

RISCOS E COMPLICAÇÕES POSSÍVEIS: Fui informado(a) sobre os possíveis riscos e complicações, que incluem:
- Edema (inchaço) e equimoses (manchas roxas) pós-operatórios
- Trismo (dificuldade de abertura bucal) temporário
- Dor pós-operatória de intensidade variável
- Parestesia (dormência) temporária ou permanente por proximidade com nervos
- Comunicação buco-sinusal (abertura entre a boca e o seio maxilar) em casos específicos
- Alveolite (inflamação do alvéolo após a extração)
- Fratura de instrumentos ou de estruturas ósseas (raras)
- Complicações relacionadas à anestesia

CUIDADOS PÓS-OPERATÓRIOS: Comprometo-me a seguir todas as orientações pós-operatórias, incluindo:
- Repouso nas primeiras 24-48 horas
- Aplicação de compressas frias nas primeiras 48 horas
- Tomar as medicações prescritas nos horários indicados
- Seguir as orientações de higiene bucal
- Seguir a dieta recomendada
- Não realizar esforços físicos intensos por 7 dias
- Comparecer às consultas de acompanhamento

ALTERNATIVAS: Fui informado(a) sobre as alternativas à extração, incluindo acompanhamento periódico em casos específicos, bem como sobre as consequências de não realizar o tratamento recomendado.

AUTORIZAÇÃO: Declaro que li e entendi todas as informações contidas neste termo e que todas as minhas dúvidas foram esclarecidas. Autorizo o procedimento proposto, bem como intervenções adicionais que se façam necessárias durante o ato cirúrgico.

[LOCAL E DATA]

_______________________
Assinatura do Paciente

_______________________
Assinatura do Profissional`,
      lastUpdated: '2025-03-18',
      required: true,
    },
    {
      id: '5',
      title: 'Consentimento para procedimento de preenchimento com ácido hialurônico',
      description: 'Documento para autorização de procedimentos estéticos com preenchedores',
      category: 'Harmonização',
      content: `TERMO DE CONSENTIMENTO LIVRE E ESCLARECIDO PARA PROCEDIMENTO DE PREENCHIMENTO COM ÁCIDO HIALURÔNICO

Eu, [NOME DO PACIENTE], declaro que fui informado(a) pelo(a) Dr(a). [NOME DO PROFISSIONAL] sobre o procedimento de preenchimento com ácido hialurônico, seus riscos, benefícios e alternativas.

PROCEDIMENTO: O preenchimento com ácido hialurônico é um procedimento minimamente invasivo que consiste na aplicação de gel biocompatível à base de ácido hialurônico nas regiões faciais para restaurar volume, corrigir sulcos, rugas e assimetrias, ou remodelar contornos.

ÁREAS A SEREM TRATADAS: As áreas que serão submetidas ao procedimento são: [ÁREAS ESPECÍFICAS: lábios, sulco nasolabial, região malar, etc.].

RESULTADOS ESPERADOS: Estou ciente de que os resultados são temporários (6-18 meses, dependendo da área, produto utilizado e metabolismo individual) e que podem ser necessárias aplicações adicionais para correção ou manutenção dos efeitos.

RISCOS E COMPLICAÇÕES POSSÍVEIS: Fui informado(a) sobre os possíveis efeitos colaterais e complicações, que podem incluir:
- Equimoses (roxos), edemas (inchaços) e vermelhidão no local da aplicação
- Assimetria temporária
- Dor ou desconforto no local da aplicação
- Nódulos ou irregularidades palpáveis
- Reações alérgicas (raras)
- Complicações vasculares como isquemia e necrose (extremamente raras)
- Migração do produto para áreas adjacentes

CONTRAINDICAÇÕES: Declaro que informei ao profissional todas as minhas condições de saúde e que não apresento nenhuma das seguintes contraindicações:
- Gravidez ou amamentação
- Doenças autoimunes em fase ativa
- Infecção ativa no local da aplicação
- Alergia conhecida ao ácido hialurônico ou componentes da fórmula
- Tendência a formar queloides ou cicatrizes hipertróficas

CUIDADOS PÓS-PROCEDIMENTO: Comprometo-me a seguir todas as recomendações pós-procedimento, incluindo:
- Evitar massagear ou pressionar a área tratada por 24 horas
- Não realizar atividade física intensa nas primeiras 24 horas
- Evitar exposição ao sol, sauna ou temperaturas extremas por 48 horas
- Evitar o consumo de álcool nas primeiras 24 horas
- Informar imediatamente qualquer efeito adverso incomum

AUTORIZAÇÃO: Declaro que li e entendi todas as informações contidas neste termo e que todas as minhas dúvidas foram esclarecidas. Autorizo o registro fotográfico para documentação científica e avaliação de resultados.

Portanto, dou meu consentimento livre e esclarecido para a realização do procedimento de preenchimento com ácido hialurônico.

[LOCAL E DATA]

_______________________
Assinatura do Paciente

_______________________
Assinatura do Profissional`,
      lastUpdated: '2025-04-01',
      required: true,
    },
  ];

  // Função para filtrar os itens pelo termo de busca
  const filterItems = (items: any[], query: string) => {
    if (!query) return items;
    
    return items.filter(item => 
      item.title.toLowerCase().includes(query.toLowerCase()) ||
      item.description.toLowerCase().includes(query.toLowerCase()) ||
      item.category.toLowerCase().includes(query.toLowerCase()) ||
      (item.tags && item.tags.some((tag: string) => tag.toLowerCase().includes(query.toLowerCase())))
    );
  };

  // Itens filtrados com base na busca
  const filteredMaterials = filterItems(educationalMaterials, searchQuery);
  const filteredProtocols = filterItems(clinicalProtocols, searchQuery);
  const filteredForms = filterItems(consentForms, searchQuery);

  // Função para renderizar o tipo de arquivo com ícone
  const renderFileType = (type: string) => {
    switch (type) {
      case 'pdf':
        return <FileText className="h-4 w-4 text-red-500" />;
      case 'video':
        return <Info className="h-4 w-4 text-blue-500" />;
      case 'image':
        return <Info className="h-4 w-4 text-purple-500" />;
      case 'text':
        return <BookOpen className="h-4 w-4 text-green-500" />;
      default:
        return <FileText className="h-4 w-4" />;
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row gap-4 items-center justify-between mb-6">
        <div className="relative w-full md:max-w-sm">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            type="search"
            placeholder="Buscar materiais, protocolos e formulários..."
            className="pl-8"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
        <Button variant="outline" className="w-full md:w-auto">
          <FilePlus className="mr-2 h-4 w-4" />
          Adicionar novo
        </Button>
      </div>

      <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
        <TabsList className="grid w-full grid-cols-3">
          <TabsTrigger value="materials">
            <BookOpen className="h-4 w-4 mr-2" />
            Materiais Educativos
          </TabsTrigger>
          <TabsTrigger value="protocols">
            <FileCheck className="h-4 w-4 mr-2" />
            Protocolos Clínicos
          </TabsTrigger>
          <TabsTrigger value="forms">
            <FileText className="h-4 w-4 mr-2" />
            Formulários
          </TabsTrigger>
        </TabsList>

        {/* Materiais Educativos */}
        <TabsContent value="materials">
          <Card>
            <CardHeader>
              <CardTitle>Materiais Educativos para Pacientes</CardTitle>
              <CardDescription>
                Documentos e recursos para orientação e informação dos pacientes
              </CardDescription>
            </CardHeader>
            <CardContent>
              {searchQuery && (
                <div className="mb-4">
                  <p className="text-sm text-muted-foreground">
                    {filteredMaterials.length} resultados encontrados para "{searchQuery}"
                  </p>
                </div>
              )}
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {filteredMaterials.map((material) => (
                  <Card key={material.id} className="overflow-hidden">
                    <CardHeader className="p-4 pb-2">
                      <div className="flex items-start justify-between">
                        <div className="flex items-center">
                          {renderFileType(material.type)}
                          <CardTitle className="text-md ml-2">{material.title}</CardTitle>
                        </div>
                        <Badge variant="outline">{material.category}</Badge>
                      </div>
                    </CardHeader>
                    <CardContent className="p-4 pt-0">
                      <p className="text-sm text-muted-foreground mb-2">{material.description}</p>
                      <div className="flex flex-wrap gap-1 mt-2">
                        {material.tags.map((tag, index) => (
                          <Badge key={index} variant="secondary" className="text-xs">
                            {tag}
                          </Badge>
                        ))}
                      </div>
                    </CardContent>
                    <CardFooter className="p-4 pt-0 flex justify-between items-center">
                      <span className="text-xs text-muted-foreground">
                        Adicionado em {new Date(material.dateAdded).toLocaleDateString('pt-BR')}
                      </span>
                      <Button variant="outline" size="sm">
                        <Download className="h-4 w-4 mr-1" />
                        {material.type === 'text' ? 'Ver' : 'Baixar'}
                      </Button>
                    </CardFooter>
                  </Card>
                ))}
              </div>
              {filteredMaterials.length === 0 && (
                <div className="text-center py-8">
                  <p className="text-muted-foreground">Nenhum material encontrado.</p>
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        {/* Protocolos Clínicos */}
        <TabsContent value="protocols">
          <Card>
            <CardHeader>
              <CardTitle>Protocolos Clínicos e Procedimentos</CardTitle>
              <CardDescription>
                Procedimentos padronizados e passos detalhados para tratamentos
              </CardDescription>
            </CardHeader>
            <CardContent>
              {searchQuery && (
                <div className="mb-4">
                  <p className="text-sm text-muted-foreground">
                    {filteredProtocols.length} resultados encontrados para "{searchQuery}"
                  </p>
                </div>
              )}
              <div className="space-y-4">
                {filteredProtocols.map((protocol) => (
                  <Accordion type="single" collapsible key={protocol.id}>
                    <AccordionItem value={protocol.id}>
                      <AccordionTrigger className="px-4 py-2 bg-muted/30 rounded-lg">
                        <div className="flex items-center text-left">
                          <div className="mr-4">
                            <Badge variant="outline">{protocol.category}</Badge>
                          </div>
                          <div>
                            <h3 className="text-sm font-medium">{protocol.title}</h3>
                            <p className="text-xs text-muted-foreground">{protocol.description}</p>
                          </div>
                        </div>
                      </AccordionTrigger>
                      <AccordionContent className="px-4 pt-2 pb-4">
                        <div className="space-y-4">
                          <div>
                            <h4 className="font-medium text-sm mb-2">Etapas do Procedimento</h4>
                            <ol className="space-y-2">
                              {protocol.steps.map((step) => (
                                <li key={step.step} className="flex gap-2">
                                  <div className="rounded-full bg-primary/10 text-primary w-6 h-6 flex items-center justify-center text-xs flex-shrink-0">
                                    {step.step}
                                  </div>
                                  <div>
                                    <p className="text-sm">{step.instruction}</p>
                                    {step.notes && (
                                      <p className="text-xs text-muted-foreground">{step.notes}</p>
                                    )}
                                  </div>
                                </li>
                              ))}
                            </ol>
                          </div>
                          <div>
                            <h4 className="font-medium text-sm mb-2">Materiais Necessários</h4>
                            <ul className="grid grid-cols-1 md:grid-cols-2 gap-1">
                              {protocol.materials.map((material, index) => (
                                <li key={index} className="text-sm flex items-center gap-1">
                                  <span className="text-primary">•</span>
                                  {material}
                                </li>
                              ))}
                            </ul>
                          </div>
                          <div className="text-xs text-muted-foreground">
                            Última atualização: {new Date(protocol.lastUpdated).toLocaleDateString('pt-BR')}
                          </div>
                        </div>
                      </AccordionContent>
                    </AccordionItem>
                  </Accordion>
                ))}
                {filteredProtocols.length === 0 && (
                  <div className="text-center py-8">
                    <p className="text-muted-foreground">Nenhum protocolo encontrado.</p>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Formulários de Consentimento */}
        <TabsContent value="forms">
          <Card>
            <CardHeader>
              <CardTitle>Formulários de Consentimento</CardTitle>
              <CardDescription>
                Documentos e termos de consentimento para procedimentos
              </CardDescription>
            </CardHeader>
            <CardContent>
              {searchQuery && (
                <div className="mb-4">
                  <p className="text-sm text-muted-foreground">
                    {filteredForms.length} resultados encontrados para "{searchQuery}"
                  </p>
                </div>
              )}
              <div className="space-y-4">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Título</TableHead>
                      <TableHead>Categoria</TableHead>
                      <TableHead>Atualização</TableHead>
                      <TableHead>Obrigatório</TableHead>
                      <TableHead className="text-right">Ações</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {filteredForms.map((form) => (
                      <TableRow key={form.id}>
                        <TableCell className="font-medium">{form.title}</TableCell>
                        <TableCell>
                          <Badge variant="outline">{form.category}</Badge>
                        </TableCell>
                        <TableCell>{new Date(form.lastUpdated).toLocaleDateString('pt-BR')}</TableCell>
                        <TableCell>
                          {form.required ? (
                            <Badge variant="default" className="bg-emerald-500 hover:bg-emerald-600">Sim</Badge>
                          ) : (
                            <Badge variant="outline">Não</Badge>
                          )}
                        </TableCell>
                        <TableCell className="text-right">
                          <div className="flex justify-end gap-2">
                            <Button variant="outline" size="sm">
                              <Bookmark className="h-4 w-4" />
                              <span className="sr-only">Salvar</span>
                            </Button>
                            <Button variant="outline" size="sm">
                              <Download className="h-4 w-4" />
                              <span className="sr-only">Baixar</span>
                            </Button>
                          </div>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
                {filteredForms.length > 0 && (
                  <Accordion type="single" collapsible>
                    <AccordionItem value="form-preview">
                      <AccordionTrigger className="px-4 py-2 bg-muted/30 rounded-lg">
                        Visualizar exemplo de formulário
                      </AccordionTrigger>
                      <AccordionContent className="px-4 py-2">
                        <div className="rounded-lg border p-4 bg-white dark:bg-muted/30">
                          <ScrollArea className="h-[400px] w-full rounded-md pr-4">
                            <div className="text-sm whitespace-pre-wrap font-mono">
                              {consentForms[0].content}
                            </div>
                          </ScrollArea>
                        </div>
                      </AccordionContent>
                    </AccordionItem>
                  </Accordion>
                )}
                {filteredForms.length === 0 && (
                  <div className="text-center py-8">
                    <p className="text-muted-foreground">Nenhum formulário encontrado.</p>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
};

export default KnowledgeBase;