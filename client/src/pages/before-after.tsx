import React from "react";
import { Card } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Smile, Frown, Sparkles, Zap, Check, Heart, Star, FolderHeart, Crown, BadgeAlert } from "lucide-react";

// Componente para mostrar antes e depois sem imagens externas
function BeforeAfterCard({
  title,
  description,
  beforeIcon,
  afterIcon,
  beforeTitle,
  afterTitle,
  beforeDescription,
  afterDescription,
  patientName,
  patientAge,
  testimonial,
}: {
  title: string;
  description: string;
  beforeIcon: React.ReactNode;
  afterIcon: React.ReactNode;
  beforeTitle: string;
  afterTitle: string;
  beforeDescription: string;
  afterDescription: string;
  patientName: string;
  patientAge: number;
  testimonial: string;
}) {
  return (
    <Card className="mb-8 overflow-hidden shadow-lg border-0">
      <div className="p-4 md:p-6 flex flex-col">
        {/* Título e descrição */}
        <div className="mb-6">
          <h3 className="text-2xl md:text-3xl font-bold bg-gradient-to-r from-primary to-primary/70 text-transparent bg-clip-text mb-2">
            {title}
          </h3>
          <p className="text-muted-foreground text-sm md:text-base">{description}</p>
        </div>

        {/* Representação visual de antes/depois com ícones */}
        <div className="flex flex-col md:flex-row gap-6 mb-6">
          <div className="flex-1 flex flex-col border rounded-lg overflow-hidden shadow-md">
            <div className="bg-destructive/20 p-3 text-center font-medium text-destructive text-sm uppercase tracking-wider">
              Antes
            </div>
            <div className="h-48 sm:h-64 md:h-80 flex flex-col items-center justify-center p-4 bg-gradient-to-b from-slate-50 to-slate-100">
              <div className="text-red-500 mb-4 p-6 rounded-full bg-red-100 border border-red-200" style={{fontSize: "48px"}}>
                {beforeIcon}
              </div>
              <h4 className="text-xl font-bold text-red-600 mb-2">{beforeTitle}</h4>
              <p className="text-center text-muted-foreground">{beforeDescription}</p>
            </div>
          </div>
          
          <div className="flex-1 flex flex-col border rounded-lg overflow-hidden shadow-md">
            <div className="bg-primary/20 p-3 text-center font-medium text-primary text-sm uppercase tracking-wider">
              Depois
            </div>
            <div className="h-48 sm:h-64 md:h-80 flex flex-col items-center justify-center p-4 bg-gradient-to-b from-slate-50 to-slate-100">
              <div className="text-green-500 mb-4 p-6 rounded-full bg-green-100 border border-green-200" style={{fontSize: "48px"}}>
                {afterIcon}
              </div>
              <h4 className="text-xl font-bold text-green-600 mb-2">{afterTitle}</h4>
              <p className="text-center text-muted-foreground">{afterDescription}</p>
            </div>
          </div>
        </div>
        
        {/* Depoimento */}
        <div className="bg-primary/5 p-5 rounded-xl mb-6">
          <div className="flex items-center gap-4 mb-4">
            <Avatar className="h-16 w-16 border-2 border-primary shadow-md">
              <AvatarFallback className="text-lg font-bold bg-primary/10 text-primary">
                {patientName.charAt(0)}
              </AvatarFallback>
            </Avatar>
            <div>
              <h4 className="font-bold text-lg">{patientName}</h4>
              <p className="text-sm text-muted-foreground">{patientAge} anos</p>
            </div>
          </div>
          
          <div className="relative mx-4">
            <div className="absolute -left-6 -top-6 text-7xl text-primary/20">"</div>
            <blockquote className="italic text-base md:text-lg relative z-10 px-4 py-2">
              {testimonial}
            </blockquote>
            <div className="absolute -right-6 -bottom-6 text-7xl text-primary/20">"</div>
          </div>
        </div>
        
        {/* CTA */}
        <div className="bg-gradient-to-r from-primary to-primary/80 p-6 rounded-xl shadow-lg text-white text-center">
          <h4 className="font-bold text-xl mb-3">Quer resultados como esse?</h4>
          <p className="mb-5">Agende uma consulta de avaliação GRATUITA hoje mesmo!</p>
          <button className="bg-white text-primary hover:bg-primary-foreground px-8 py-3 rounded-full font-bold text-lg transition-all duration-300 transform hover:scale-105 shadow-md">
            AGENDAR AGORA
          </button>
        </div>
      </div>
    </Card>
  );
}

export default function BeforeAfterPage() {
  return (
    <div className="container mx-auto py-8">
      <div className="text-center mb-10">
        <h1 className="text-3xl md:text-5xl font-extrabold mb-4 bg-gradient-to-r from-primary to-primary/60 text-transparent bg-clip-text">
          Transformações Reais
        </h1>
        <p className="text-lg text-muted-foreground max-w-2xl mx-auto px-4">
          Veja o impacto que nossos tratamentos podem ter no seu sorriso e autoestima!
          Resultados surpreendentes realizados pela nossa equipe de especialistas.
        </p>
      </div>
      
      <Tabs defaultValue="teeth-whitening" className="w-full">
        <div className="relative mb-8">
          <div className="absolute inset-0 bg-gradient-to-r from-primary/20 to-primary/5 rounded-full blur-xl opacity-70 -z-10"></div>
          <TabsList className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 p-1 rounded-full bg-background/80 backdrop-blur-sm border shadow-md">
            <TabsTrigger value="teeth-whitening" className="rounded-full data-[state=active]:bg-primary data-[state=active]:text-primary-foreground py-2">Clareamento</TabsTrigger>
            <TabsTrigger value="botox" className="rounded-full data-[state=active]:bg-primary data-[state=active]:text-primary-foreground py-2">Botox</TabsTrigger>
            <TabsTrigger value="dental-implant" className="rounded-full data-[state=active]:bg-primary data-[state=active]:text-primary-foreground py-2">Implantes</TabsTrigger>
            <TabsTrigger value="veneers" className="rounded-full data-[state=active]:bg-primary data-[state=active]:text-primary-foreground py-2">Lentes</TabsTrigger>
            <TabsTrigger value="facial-harmony" className="rounded-full data-[state=active]:bg-primary data-[state=active]:text-primary-foreground py-2">Harmonização</TabsTrigger>
          </TabsList>
        </div>
        
        <TabsContent value="teeth-whitening">
          <BeforeAfterCard
            title="Clareamento Dental Profissional"
            description="Nosso tratamento de clareamento dental avançado pode clarear seus dentes em até 8 tons em uma única sessão de consultório. Utilizamos um sistema de ativação por luz LED de última geração que potencializa o efeito do gel clareador sem causar sensibilidade."
            beforeIcon={<Frown size={64} />}
            afterIcon={<Smile size={64} />}
            beforeTitle="Coloração Amarelada"
            afterTitle="Branco Natural"
            beforeDescription="Dentes com manchas e coloração amarelada devido ao consumo de café, chá e outras substâncias que escurecem o esmalte."
            afterDescription="Dentes visivelmente mais brancos e brilhantes após apenas uma sessão de clareamento profissional em consultório."
            patientName="Mariana Silva"
            patientAge={32}
            testimonial="Eu sempre tive vergonha do meu sorriso por causa da coloração amarelada dos meus dentes. Após uma única sessão de clareamento, meus dentes ficaram incrivelmente brancos! Agora sorrio com total confiança em todas as fotos. O procedimento foi super rápido e indolor, exatamente como me prometeram!"
          />
        </TabsContent>
        
        <TabsContent value="botox">
          <BeforeAfterCard
            title="Botox para Suavização de Rugas"
            description="Nossa aplicação de toxina botulínica é realizada por profissionais especializados que garantem resultados naturais. O procedimento é minimamente invasivo e dura apenas 30 minutos, com resultados que podem durar até 6 meses."
            beforeIcon={<BadgeAlert size={64} />}
            afterIcon={<Sparkles size={64} />}
            beforeTitle="Rugas e Linhas de Expressão"
            afterTitle="Pele Lisa e Jovem"
            beforeDescription="Rugas de expressão visíveis, especialmente na testa, ao redor dos olhos e entre as sobrancelhas."
            afterDescription="Pele visivelmente mais lisa, com suavização das rugas e aparência rejuvenescida, mantendo a naturalidade das expressões."
            patientName="Luciana Pires"
            patientAge={52}
            testimonial="Buscava um tratamento que diminuísse minhas rugas sem deixar aquele aspecto artificial. O resultado foi exatamente o que procurava! Colegas de trabalho perguntam se estou de férias ou descansada e ninguém percebe que fiz o procedimento."
          />
        </TabsContent>
        
        <TabsContent value="dental-implant">
          <BeforeAfterCard
            title="Implantes Dentários Permanentes"
            description="Nossos implantes dentários de titânio substituem completamente dentes perdidos, devolvendo função e estética ao seu sorriso. A técnica minimamente invasiva garante recuperação rápida e resultados duradouros."
            beforeIcon={<Frown size={64} />}
            afterIcon={<Crown size={64} />}
            beforeTitle="Dente Ausente"
            afterTitle="Implante Perfeito"
            beforeDescription="Espaço vazio na arcada dentária causando dificuldades para mastigar e constrangimento social."
            afterDescription="Implante dental permanente que se integra completamente ao osso e parece um dente natural."
            patientName="Carlos Mendonça"
            patientAge={48}
            testimonial="Perdi um dente da frente em um acidente e fiquei muito constrangido por anos. O implante que recebi na clínica ficou tão natural que ninguém percebe que não é meu dente original. Recuperei minha confiança para sorrir!"
          />
        </TabsContent>
        
        <TabsContent value="veneers">
          <BeforeAfterCard
            title="Lentes de Contato Dental"
            description="Nossas lentes de contato dental (facetas) são ultrafinas e proporcionam um sorriso perfeito. Corrigem dentes manchados, quebrados, desalinhados ou com espaços, criando um sorriso harmonioso e natural."
            beforeIcon={<BadgeAlert size={64} />}
            afterIcon={<Star size={64} />}
            beforeTitle="Dentes Desalinhados"
            afterTitle="Sorriso Perfeitamente Alinhado"
            beforeDescription="Dentes com espaçamento irregular, manchas permanentes ou pequenas fraturas que comprometem a estética."
            afterDescription="Sorriso perfeitamente alinhado e branco com lentes de contato dental ultrafinas e resistentes."
            patientName="Paula Gonzaga"
            patientAge={35}
            testimonial="Sempre tive dentes tortos e com espaçamentos que me incomodavam muito. Com as lentes de contato dental, meu sorriso ficou alinhado e perfeito em apenas duas sessões! O resultado superou todas as minhas expectativas."
          />
        </TabsContent>
        
        <TabsContent value="facial-harmony">
          <BeforeAfterCard
            title="Harmonização Facial Completa"
            description="Nossa abordagem de harmonização facial combina diferentes técnicas como botox, preenchimentos e bioestimuladores para equilibrar as proporções do rosto, realçando a beleza natural e rejuvenescendo a aparência."
            beforeIcon={<FolderHeart size={64} />}
            afterIcon={<Heart size={64} />}
            beforeTitle="Proporções Assimétricas"
            afterTitle="Harmonia Facial Perfeita"
            beforeDescription="Assimetrias faciais, volume insuficiente em áreas estratégicas e sinais de envelhecimento."
            afterDescription="Contornos faciais equilibrados, volume distribuído harmonicamente e aparência rejuvenescida e natural."
            patientName="Fernanda Lopes"
            patientAge={42}
            testimonial="A harmonização facial transformou minha autoestima! O médico analisou meu rosto como um todo e aplicou as técnicas nos pontos certos. O resultado é natural e equilibrado, exatamente o que eu queria. Todos dizem que estou com aparência rejuvenescida, mas ninguém sabe exatamente o que mudou!"
          />
        </TabsContent>
      </Tabs>
    </div>
  );
}