import React from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";

// URLs de imagens SVG criadas localmente (garantia de funcionamento)
const teethWhiteningImageUrl = "/teeth-before-after.svg";
const botoxImageUrl = "/botox-before-after.svg";

// Componente para mostrar antes e depois
function BeforeAfterCard({
  title,
  description,
  beforeImage,
  afterImage,
  patientName,
  patientAge,
  testimonial,
}: {
  title: string;
  description: string;
  beforeImage: string;
  afterImage: string;
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

        {/* Imagens antes/depois */}
        <div className="flex flex-col md:flex-row gap-6 mb-6">
          <div className="flex-1 flex flex-col border rounded-lg overflow-hidden shadow-md">
            <div className="bg-destructive/20 p-3 text-center font-medium text-destructive text-sm uppercase tracking-wider">
              Antes
            </div>
            <div className="h-48 sm:h-64 md:h-80 overflow-hidden">
              <img
                src={beforeImage}
                alt={`${title} antes`}
                className="w-full h-full object-cover"
              />
            </div>
          </div>
          
          <div className="flex-1 flex flex-col border rounded-lg overflow-hidden shadow-md">
            <div className="bg-primary/20 p-3 text-center font-medium text-primary text-sm uppercase tracking-wider">
              Depois
            </div>
            <div className="h-48 sm:h-64 md:h-80 overflow-hidden">
              <img
                src={afterImage}
                alt={`${title} depois`}
                className="w-full h-full object-cover"
              />
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
          <Card className="mb-8 overflow-hidden shadow-lg border-0">
            <div className="p-4 md:p-6 flex flex-col">
              {/* Título e descrição */}
              <div className="mb-6">
                <h3 className="text-2xl md:text-3xl font-bold bg-gradient-to-r from-primary to-primary/70 text-transparent bg-clip-text mb-2">
                  Clareamento Dental Profissional
                </h3>
                <p className="text-muted-foreground text-sm md:text-base">
                  Nosso tratamento de clareamento dental avançado pode clarear seus dentes em até 8 tons em uma única sessão 
                  de consultório. Utilizamos um sistema de ativação por luz LED de última geração que potencializa o efeito do 
                  gel clareador sem causar sensibilidade.
                </p>
              </div>

              {/* Imagens antes/depois - Clareamento real */}
              <div>
                <div className="w-full flex items-center justify-center border rounded-lg overflow-hidden shadow-md mb-6">
                  <img
                    src={teethWhiteningImageUrl}
                    alt="Antes e depois do clareamento dental"
                    className="w-full h-auto object-contain"
                  />
                </div>
                
                {/* Instruções sobre a imagem */}
                <div className="bg-primary/5 p-4 rounded-lg mb-8">
                  <div className="flex items-start gap-3">
                    <div className="bg-destructive/20 p-2 rounded font-medium text-destructive text-sm">ANTES</div>
                    <p className="text-sm">Dentes com coloração amarelada devido ao consumo de café, chá e outras substâncias que mancham o esmalte ao longo do tempo.</p>
                  </div>
                  <div className="w-full border-t border-primary/10 my-3"></div>
                  <div className="flex items-start gap-3">
                    <div className="bg-primary/20 p-2 rounded font-medium text-primary text-sm">DEPOIS</div>
                    <p className="text-sm">Dentes visivelmente mais brancos após apenas uma sessão de clareamento profissional, realizado em consultório com segurança e conforto.</p>
                  </div>
                </div>
              </div>
              
              {/* Depoimento */}
              <div className="bg-primary/5 p-5 rounded-xl mb-6">
                <div className="flex items-center gap-4 mb-4">
                  <Avatar className="h-16 w-16 border-2 border-primary shadow-md">
                    <AvatarFallback className="text-lg font-bold bg-primary/10 text-primary">
                      M
                    </AvatarFallback>
                  </Avatar>
                  <div>
                    <h4 className="font-bold text-lg">Mariana Silva</h4>
                    <p className="text-sm text-muted-foreground">32 anos</p>
                  </div>
                </div>
                
                <div className="relative mx-4">
                  <div className="absolute -left-6 -top-6 text-7xl text-primary/20">"</div>
                  <blockquote className="italic text-base md:text-lg relative z-10 px-4 py-2">
                    Eu sempre tive vergonha do meu sorriso por causa da coloração amarelada dos meus dentes. Após uma única sessão de clareamento, meus dentes ficaram incrivelmente brancos! Agora sorrio com total confiança em todas as fotos. O procedimento foi super rápido e indolor, exatamente como me prometeram!
                  </blockquote>
                  <div className="absolute -right-6 -bottom-6 text-7xl text-primary/20">"</div>
                </div>
              </div>
              
              {/* CTA */}
              <div className="bg-gradient-to-r from-primary to-primary/80 p-6 rounded-xl shadow-lg text-white text-center">
                <h4 className="font-bold text-xl mb-3">Quer um sorriso branquinho como esse?</h4>
                <p className="mb-5">Agende uma consulta de avaliação GRATUITA hoje mesmo!</p>
                <button className="bg-white text-primary hover:bg-primary-foreground px-8 py-3 rounded-full font-bold text-lg transition-all duration-300 transform hover:scale-105 shadow-md">
                  AGENDAR AGORA
                </button>
              </div>
            </div>
          </Card>
        </TabsContent>
        
        <TabsContent value="botox">
          <Card className="mb-8 overflow-hidden shadow-lg border-0">
            <div className="p-4 md:p-6 flex flex-col">
              {/* Título e descrição */}
              <div className="mb-6">
                <h3 className="text-2xl md:text-3xl font-bold bg-gradient-to-r from-primary to-primary/70 text-transparent bg-clip-text mb-2">
                  Botox para Suavização de Rugas
                </h3>
                <p className="text-muted-foreground text-sm md:text-base">
                  Nossa aplicação de toxina botulínica é realizada por profissionais especializados que garantem resultados naturais.
                  O procedimento é minimamente invasivo e dura apenas 30 minutos, com resultados que podem durar até 6 meses.
                </p>
              </div>

              {/* Imagens antes/depois - Masculino e Feminino */}
              <div className="space-y-8">
                {/* Resultados masculinos e femininos */}
                <div>
                  <h4 className="text-xl font-bold mb-3">Resultados reais</h4>
                  <div className="w-full aspect-video flex items-center justify-center">
                    <img
                      src={botoxImageUrl}
                      alt="Botox antes e depois em homens e mulheres"
                      className="w-full h-auto object-contain shadow-md rounded-lg"
                    />
                  </div>
                </div>
                
                {/* Explicação da imagem */}
                <div className="bg-primary/5 p-4 rounded-lg mt-4">
                  <div className="flex items-start gap-3 mb-3">
                    <div className="bg-destructive/20 p-2 rounded font-medium text-destructive text-sm">ANTES</div>
                    <p className="text-sm">Rugas de expressão e sinais de envelhecimento visíveis. Aparência cansada e marcas naturais do tempo.</p>
                  </div>
                  <div className="flex items-start gap-3">
                    <div className="bg-primary/20 p-2 rounded font-medium text-primary text-sm">DEPOIS</div>
                    <p className="text-sm">Pele visivelmente mais lisa, com suavização das rugas e aparência rejuvenescida, mantendo a naturalidade das expressões.</p>
                  </div>
                </div>
              </div>
              
              {/* CTA */}
              <div className="bg-gradient-to-r from-primary to-primary/80 p-6 rounded-xl shadow-lg text-white text-center mt-8">
                <h4 className="font-bold text-xl mb-3">Quer resultados como esses?</h4>
                <p className="mb-5">Agende uma consulta de avaliação GRATUITA hoje mesmo!</p>
                <button className="bg-white text-primary hover:bg-primary-foreground px-8 py-3 rounded-full font-bold text-lg transition-all duration-300 transform hover:scale-105 shadow-md">
                  AGENDAR AGORA
                </button>
              </div>
            </div>
          </Card>
        </TabsContent>
        
        <TabsContent value="dental-implant">
          <BeforeAfterCard
            title="Implantes Dentários Permanentes"
            description="Nossos implantes dentários de titânio substituem completamente dentes perdidos, devolvendo função e estética ao seu sorriso. A técnica minimamente invasiva garante recuperação rápida e resultados duradouros."
            beforeImage="/dental-implant-before.svg"
            afterImage="/dental-implant-after.svg"
            patientName="Carlos Mendonça"
            patientAge={48}
            testimonial="Perdi um dente da frente em um acidente e fiquei muito constrangido por anos. O implante que recebi na clínica ficou tão natural que ninguém percebe que não é meu dente original. Recuperei minha confiança para sorrir!"
          />
        </TabsContent>
        
        <TabsContent value="veneers">
          <BeforeAfterCard
            title="Lentes de Contato Dental"
            description="Nossas lentes de contato dental (facetas) são ultrafinas e proporcionam um sorriso perfeito. Corrigem dentes manchados, quebrados, desalinhados ou com espaços, criando um sorriso harmonioso e natural."
            beforeImage="/veneers-before.svg"
            afterImage="/veneers-after.svg"
            patientName="Paula Gonzaga"
            patientAge={35}
            testimonial="Sempre tive dentes tortos e com espaçamentos que me incomodavam muito. Com as lentes de contato dental, meu sorriso ficou alinhado e perfeito em apenas duas sessões! O resultado superou todas as minhas expectativas."
          />
        </TabsContent>
        
        <TabsContent value="facial-harmony">
          <BeforeAfterCard
            title="Harmonização Facial Completa"
            description="Nossa abordagem de harmonização facial combina diferentes técnicas como botox, preenchimentos e bioestimuladores para equilibrar as proporções do rosto, realçando a beleza natural e rejuvenescendo a aparência."
            beforeImage="/harmony-before.svg"
            afterImage="/harmony-after.svg"
            patientName="Fernanda Lopes"
            patientAge={42}
            testimonial="A harmonização facial transformou minha autoestima! O médico analisou meu rosto como um todo e aplicou as técnicas nos pontos certos. O resultado é natural e equilibrado, exatamente o que eu queria. Todos dizem que estou com aparência rejuvenescida, mas ninguém sabe exatamente o que mudou!"
          />
        </TabsContent>
      </Tabs>
    </div>
  );
}