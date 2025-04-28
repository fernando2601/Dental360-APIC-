import { useState, useEffect } from "react";
import { Link, useLocation } from "wouter";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Separator } from "@/components/ui/separator";
import {
  Calendar,
  Users,
  Package,
  DollarSign,
  FileText,
  UserCircle,
  Settings,
  Home,
  BarChart2,
  ChevronLeft,
  ChevronRight,
  ChevronDown,
  ChevronUp,
  MessageSquare,
  Zap,
  Building,
  Gift,
  BadgePercent,
  Images,
  LayoutDashboard,
  LineChart,
  Receipt,
  BarChart,
  TrendingUp
} from "lucide-react";
import { FaWhatsapp } from "react-icons/fa";
import { useMobile } from "@/hooks/use-mobile";
import { cn } from "@/lib/utils";

type SidebarProps = {
  className?: string;
};

export function Sidebar({ className }: SidebarProps) {
  const [collapsed, setCollapsed] = useState(false);
  const [location] = useLocation();
  const isMobile = useMobile();

  useEffect(() => {
    if (isMobile) {
      setCollapsed(true);
    } else {
      setCollapsed(false);
    }
  }, [isMobile]);

  return (
    <aside
      className={cn(
        "group border-r bg-card transition-all duration-300 ease-in-out",
        collapsed ? "w-[70px]" : "w-[240px]",
        className
      )}
    >
      <div className="flex h-full flex-col">
        <div className="flex h-14 items-center border-b px-3">
          <div className={cn("flex items-center gap-2", collapsed && "justify-center w-full")}>
            <Zap className="h-6 w-6 text-primary" />
            {!collapsed && <span className="font-heading font-bold text-xl">DentalSpa</span>}
          </div>
          <Button
            variant="ghost"
            size="icon"
            className={cn("ml-auto h-8 w-8", collapsed && "hidden group-hover:flex absolute right-2")}
            onClick={() => setCollapsed(!collapsed)}
          >
            {collapsed ? <ChevronRight className="h-4 w-4" /> : <ChevronLeft className="h-4 w-4" />}
          </Button>
        </div>
        <ScrollArea className="flex-1">
          <nav className="flex flex-col gap-2 p-2">
            <div className="px-4 py-2">
              <h3 className={cn("text-xs font-medium text-muted-foreground", collapsed && "sr-only")}>
                Principal
              </h3>
            </div>
            <NavLink href="/" icon={<Home className="h-4 w-4" />} label="Início" collapsed={collapsed} active={location === "/"} />
            <NavLink href="/appointments" icon={<Calendar className="h-4 w-4" />} label="Agendamentos" collapsed={collapsed} active={location === "/appointments"} />
            <NavLink href="/clients" icon={<Users className="h-4 w-4" />} label="Pacientes" collapsed={collapsed} active={location === "/clients"} />
            <NavLink href="/inventory" icon={<Package className="h-4 w-4" />} label="Estoque" collapsed={collapsed} active={location === "/inventory"} />
            
            <NavSubmenu 
              icon={<DollarSign className="h-4 w-4" />} 
              label="Finanças" 
              collapsed={collapsed} 
              active={location.startsWith("/finances")}
            >
              <SubNavLink 
                href="/finances?tab=dashboard" 
                icon={<LayoutDashboard className="h-4 w-4" />} 
                label="Dashboard" 
                active={location === "/finances" || location === "/finances?tab=dashboard"} 
              />
              <SubNavLink 
                href="/finances?tab=cash-flow" 
                icon={<LineChart className="h-4 w-4" />} 
                label="Fluxo de Caixa" 
                active={location === "/finances?tab=cash-flow"} 
              />
              <SubNavLink 
                href="/finances?tab=transactions" 
                icon={<Receipt className="h-4 w-4" />} 
                label="Transações" 
                active={location === "/finances?tab=transactions"} 
              />
              <SubNavLink 
                href="/finances?tab=expenses" 
                icon={<BarChart className="h-4 w-4" />} 
                label="Despesas" 
                active={location === "/finances?tab=expenses"} 
              />
              <SubNavLink 
                href="/finances?tab=projections" 
                icon={<TrendingUp className="h-4 w-4" />} 
                label="Projeções" 
                active={location === "/finances?tab=projections"} 
              />
            </NavSubmenu>
            
            <NavLink href="/services" icon={<FileText className="h-4 w-4" />} label="Serviços" collapsed={collapsed} active={location === "/services"} />
            <NavLink href="/staff" icon={<UserCircle className="h-4 w-4" />} label="Equipe" collapsed={collapsed} active={location === "/staff"} />
            <NavLink href="/whatsapp" icon={<FaWhatsapp className="h-4 w-4" />} label="WhatsApp" collapsed={collapsed} active={location === "/whatsapp"} />
            <NavLink href="/packages" icon={<Gift className="h-4 w-4" />} label="Pacotes" collapsed={collapsed} active={location === "/packages"} />
            <NavLink href="/subscriptions" icon={<BadgePercent className="h-4 w-4" />} label="Assinatura" collapsed={collapsed} active={location === "/subscriptions"} />
            <NavLink href="/clinic-info" icon={<Building className="h-4 w-4" />} label="Dados da Clínica" collapsed={collapsed} active={location === "/clinic-info"} />
            <NavLink href="/before-after" icon={<Images className="h-4 w-4" />} label="Antes & Depois" collapsed={collapsed} active={location === "/before-after"} />
            
            <div className="px-4 py-2 mt-4">
              <h3 className={cn("text-xs font-medium text-muted-foreground", collapsed && "sr-only")}>
                IA & Análises
              </h3>
            </div>
            <NavLink 
              href="/settings" 
              icon={<MessageSquare className="h-4 w-4" />} 
              label="Templates de Chatbot" 
              collapsed={collapsed} 
              active={location === "/settings"} 
              notification={2}
            />
            <NavLink 
              href="/analytics" 
              icon={<BarChart2 className="h-4 w-4" />} 
              label="Análises" 
              collapsed={collapsed} 
              active={location === "/analytics"} 
            />
            <NavLink 
              href="/settings?tab=privacy" 
              icon={<Settings className="h-4 w-4" />} 
              label="Configurações de Privacidade" 
              collapsed={collapsed} 
              active={location.includes("/settings?tab=privacy")} 
            />
          </nav>
        </ScrollArea>
        <Separator />
        <div className="p-4">
          <div className="flex items-center gap-2">
            <div className="h-8 w-8 rounded-full bg-primary text-primary-foreground flex items-center justify-center">
              DS
            </div>
            {!collapsed && (
              <div className="space-y-1">
                <p className="text-sm font-medium">Dr. Sarah</p>
                <p className="text-xs text-muted-foreground">Administrator</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </aside>
  );
}

interface NavLinkProps {
  href: string;
  icon: React.ReactNode;
  label: string;
  collapsed: boolean;
  active: boolean;
  notification?: number;
}

function NavLink({ href, icon, label, collapsed, active, notification }: NavLinkProps) {
  return (
    <Link href={href}>
      <div
        className={cn(
          "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-all hover:bg-accent cursor-pointer",
          active ? "bg-accent text-accent-foreground" : "text-muted-foreground hover:text-foreground",
          collapsed && "justify-center"
        )}
      >
        {icon}
        {!collapsed && <span>{label}</span>}
        {!collapsed && notification && (
          <Badge className="ml-auto" variant="secondary">
            {notification}
          </Badge>
        )}
        {collapsed && notification && (
          <Badge className="absolute right-2 top-1" variant="secondary">
            {notification}
          </Badge>
        )}
      </div>
    </Link>
  );
}

interface NavSubmenuProps {
  icon: React.ReactNode;
  label: string;
  collapsed: boolean;
  active: boolean;
  children: React.ReactNode;
}

function NavSubmenu({ icon, label, collapsed, active, children }: NavSubmenuProps) {
  const [isOpen, setIsOpen] = useState(active);

  return (
    <div className="space-y-1">
      <div
        className={cn(
          "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-all hover:bg-accent cursor-pointer",
          active ? "bg-accent text-accent-foreground" : "text-muted-foreground hover:text-foreground",
          collapsed && "justify-center"
        )}
        onClick={() => !collapsed && setIsOpen(!isOpen)}
      >
        {icon}
        {!collapsed && <span>{label}</span>}
        {!collapsed && (
          <span className="ml-auto">
            {isOpen ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
          </span>
        )}
      </div>
      {!collapsed && isOpen && (
        <div className="pl-8 space-y-1">
          {children}
        </div>
      )}
    </div>
  );
}

interface SubNavLinkProps {
  href: string;
  icon: React.ReactNode;
  label: string;
  active: boolean;
}

function SubNavLink({ href, icon, label, active }: SubNavLinkProps) {
  return (
    <a href={href} className="no-underline">
      <div
        className={cn(
          "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-all hover:bg-accent cursor-pointer",
          active ? "bg-accent text-accent-foreground" : "text-muted-foreground hover:text-foreground"
        )}
      >
        {icon}
        <span>{label}</span>
      </div>
    </a>
  );
}

export default Sidebar;
