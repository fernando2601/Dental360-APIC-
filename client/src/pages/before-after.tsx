import React from "react";
import MainLayout from "@/layouts/main-layout";
import { Card, CardContent } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";

// URLs de imagens de exemplo (placeholder)
const teethWhiteningBefore = "https://placehold.co/600x400/e2e8f0/1e293b?text=Antes+Clareamento";
const teethWhiteningAfter = "https://placehold.co/600x400/e2e8f0/1e293b?text=Depois+Clareamento";
const botoxBefore = "https://placehold.co/600x400/e2e8f0/1e293b?text=Antes+Botox";
const botoxAfter = "https://placehold.co/600x400/e2e8f0/1e293b?text=Depois+Botox";
const dentalImplantBefore = "https://placehold.co/600x400/e2e8f0/1e293b?text=Antes+Implante";
const dentalImplantAfter = "https://placehold.co/600x400/e2e8f0/1e293b?text=Depois+Implante";
const veneersBefore = "https://placehold.co/600x400/e2e8f0/1e293b?text=Antes+Lentes";
const veneersAfter = "https://placehold.co/600x400/e2e8f0/1e293b?text=Depois+Lentes";
const facialHarmonyBefore = "https://placehold.co/600x400/e2e8f0/1e293b?text=Antes+Harmonização";
const facialHarmonyAfter = "https://placehold.co/600x400/e2e8f0/1e293b?text=Depois+Harmonização";

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
    <Card className="mb-8 overflow-hidden">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 p-6">
        <div className="space-y-4">
          <h3 className="text-2xl font-bold text-primary">{title}</h3>
          <p className="text-muted-foreground">{description}</p>
          
          <div className="flex flex-col border rounded-lg overflow-hidden">
            <div className="bg-destructive/10 p-2 text-center font-medium">
              ANTES
            </div>
            <div className="h-[250px] overflow-hidden flex items-center justify-center bg-muted">
              <img
                src={beforeImage}
                alt={`${title} antes`}
                className="w-full h-full object-cover"
              />
            </div>
          </div>
          
          <div className="flex flex-col border rounded-lg overflow-hidden">
            <div className="bg-primary/10 p-2 text-center font-medium">
              DEPOIS
            </div>
            <div className="h-[250px] overflow-hidden flex items-center justify-center bg-muted">
              <img
                src={afterImage}
                alt={`${title} depois`}
                className="w-full h-full object-cover"
              />
            </div>
          </div>
        </div>
        
        <div className="space-y-6 flex flex-col justify-between">
          <div className="bg-primary/5 p-6 rounded-lg space-y-4">
            <div className="flex items-center gap-4">
              <Avatar className="h-12 w-12 border-2 border-primary">
                <AvatarFallback>{patientName.charAt(0)}</AvatarFallback>
              </Avatar>
              <div>
                <h4 className="font-bold">{patientName}</h4>
                <p className="text-sm text-muted-foreground">{patientAge} anos</p>
              </div>
            </div>
            
            <div className="relative">
              <div className="absolute -left-4 -top-4 text-6xl text-primary opacity-20">"</div>
              <blockquote className="pl-6 italic text-lg relative z-10">
                {testimonial}
              </blockquote>
              <div className="absolute -right-4 bottom-0 text-6xl text-primary opacity-20">"</div>
            </div>
          </div>
          
          <div className="bg-primary text-primary-foreground p-4 rounded-lg shadow-lg">
            <h4 className="font-bold mb-2">Quer resultados como esse?</h4>
            <p className="mb-4 text-sm">Agende uma consulta de avaliação GRATUITA hoje mesmo!</p>
            <div className="flex justify-center">
              <button className="bg-white text-primary hover:bg-primary-foreground px-6 py-2 rounded-full font-bold transition-all transform hover:scale-105">
                QUERO AGENDAR AGORA
              </button>
            </div>
          </div>
        </div>
      </div>
    </Card>
  );
}

export default function BeforeAfterPage() {
  const year = new Date().getFullYear();
  
  return (
    <MainLayout>
      <div className="container mx-auto py-8">
        <div className="text-center mb-10">
          <h1 className="text-4xl font-extrabold mb-2 bg-gradient-to-r from-primary to-primary/60 text-transparent bg-clip-text">
            Antes & Depois
          </h1>
          <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
            Resultados reais de transformações realizadas pela nossa equipe. Veja o impacto que podemos ter no seu sorriso e autoestima!
          </p>
        </div>
        
        <Tabs defaultValue="teeth-whitening" className="w-full">
          <TabsList className="grid grid-cols-2 md:grid-cols-5 mb-8">
            <TabsTrigger value="teeth-whitening">Clareamento</TabsTrigger>
            <TabsTrigger value="botox">Botox</TabsTrigger>
            <TabsTrigger value="dental-implant">Implantes</TabsTrigger>
            <TabsTrigger value="veneers">Lentes</TabsTrigger>
            <TabsTrigger value="facial-harmony">Harmonização</TabsTrigger>
          </TabsList>
          
          <TabsContent value="teeth-whitening">
            <BeforeAfterCard
              title="Clareamento Dental Profissional"
              description="Nosso tratamento de clareamento dental avançado pode clarear seus dentes em até 8 tons em uma única sessão de consultório. Utilizamos um sistema de ativação por luz LED de última geração que potencializa o efeito do gel clareador sem causar sensibilidade."
              beforeImage={teethWhiteningBefore}
              afterImage={teethWhiteningAfter}
              patientName="Mariana Silva"
              patientAge={32}
              testimonial="Eu sempre tive vergonha do meu sorriso por causa da coloração amarelada dos meus dentes. Após uma única sessão de clareamento, meus dentes ficaram incrivelmente brancos! Agora sorrio com total confiança em todas as fotos. O procedimento foi super rápido e indolor, exatamente como me prometeram!"
            />
          </TabsContent>
          
          <TabsContent value="botox">
            <BeforeAfterCard
              title="Botox para Suavização de Rugas"
              description="Nossa aplicação de toxina botulínica é realizada por profissionais especializados que garantem resultados naturais. O procedimento é minimamente invasivo e dura apenas 30 minutos, com resultados que podem durar até 6 meses."
              beforeImage={botoxBefore}
              afterImage={botoxAfter}
              patientName="Roberto Mendes"
              patientAge={45}
              testimonial="As rugas de expressão na minha testa me incomodavam muito. Após a aplicação de Botox na clínica, meu rosto ficou rejuvenescido mas ainda com expressões naturais! Meus amigos notaram a diferença mas não conseguiam identificar exatamente o que eu tinha feito. Exatamente o que eu queria: um resultado discreto e elegante."
            />
          </TabsContent>
          
          <TabsContent value="dental-implant">
            <BeforeAfterCard
              title="Implante Dentário Completo"
              description="Nossos implantes dentários são feitos com titânio de alta qualidade e tecnologia de ponta para garantir uma fixação perfeita e durável. O procedimento é realizado com anestesia local e planejamento digital 3D para máxima precisão."
              beforeImage={dentalImplantBefore}
              afterImage={dentalImplantAfter}
              patientName="Carlos Oliveira"
              patientAge={58}
              testimonial="Perdi meus dentes frontais em um acidente e isso afetou completamente minha autoestima. Graças aos implantes que fiz na clínica, posso sorrir novamente! Ninguém consegue distinguir os implantes dos meus dentes naturais. O processo foi muito mais confortável do que eu imaginava e a equipe me deixou tranquilo em todas as etapas."
            />
          </TabsContent>
          
          <TabsContent value="veneers">
            <BeforeAfterCard
              title="Lentes de Contato Dental"
              description="Nossas lentes de contato dental são confeccionadas com porcelana de alta resistência e personalizadas para cada paciente. O procedimento é minimamente invasivo e transforma completamente seu sorriso em apenas duas sessões."
              beforeImage={veneersBefore}
              afterImage={veneersAfter}
              patientName="Juliana Costa"
              patientAge={29}
              testimonial="Eu tinha dentes manchados e levemente tortos que me incomodavam há anos. As lentes de contato dental transformaram completamente meu sorriso! O procedimento foi rápido e o resultado superou todas as minhas expectativas. Agora tenho o sorriso dos meus sonhos e minha confiança aumentou 100%."
            />
          </TabsContent>
          
          <TabsContent value="facial-harmony">
            <BeforeAfterCard
              title="Harmonização Facial Completa"
              description="Nossa harmonização facial combina diferentes procedimentos estéticos para equilibrar e valorizar suas características naturais. Utilizamos técnicas avançadas de preenchimento, Botox e bioestimuladores para resultados espetaculares e naturais."
              beforeImage={facialHarmonyBefore}
              afterImage={facialHarmonyAfter}
              patientName="Fernanda Souza"
              patientAge={37}
              testimonial="A harmonização facial mudou minha vida! Eu estava insatisfeita com vários aspectos do meu rosto, mas não queria parecer artificial. Os profissionais da clínica fizeram um planejamento personalizado que valorizou minhas características naturais. Agora me olho no espelho e me sinto mais bonita e confiante. Vale cada centavo investido!"
            />
          </TabsContent>
        </Tabs>
      </div>
      
      <footer className="bg-primary/5 py-4 mt-10">
        <div className="container mx-auto text-center text-sm text-muted-foreground">
          © {year} Clínica DentalSpa. Todos os direitos reservados.
        </div>
      </footer>
    </MainLayout>
  );
}