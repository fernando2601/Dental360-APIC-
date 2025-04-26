import { Link } from "wouter";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { Menu, Home, Calendar, Users, Package, DollarSign, FileText, UserCircle, Settings } from "lucide-react";
import { useState } from "react";

export function MobileNav() {
  const [open, setOpen] = useState(false);

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button variant="ghost" size="icon" className="md:hidden">
          <Menu className="h-6 w-6" />
          <span className="sr-only">Toggle Menu</span>
        </Button>
      </SheetTrigger>
      <SheetContent side="left" className="flex flex-col">
        <div className="flex items-center gap-2 border-b pb-4">
          <span className="font-bold text-lg">DentalSpa</span>
        </div>
        <nav className="flex flex-col gap-2 mt-4">
          <NavItem href="/" icon={<Home className="h-5 w-5" />} label="Dashboard" onClick={() => setOpen(false)} />
          <NavItem href="/appointments" icon={<Calendar className="h-5 w-5" />} label="Appointments" onClick={() => setOpen(false)} />
          <NavItem href="/clients" icon={<Users className="h-5 w-5" />} label="Clients" onClick={() => setOpen(false)} />
          <NavItem href="/inventory" icon={<Package className="h-5 w-5" />} label="Inventory" onClick={() => setOpen(false)} />
          <NavItem href="/finances" icon={<DollarSign className="h-5 w-5" />} label="Finances" onClick={() => setOpen(false)} />
          <NavItem href="/services" icon={<FileText className="h-5 w-5" />} label="Services" onClick={() => setOpen(false)} />
          <NavItem href="/staff" icon={<UserCircle className="h-5 w-5" />} label="Staff" onClick={() => setOpen(false)} />
          <NavItem href="/settings" icon={<Settings className="h-5 w-5" />} label="Settings" onClick={() => setOpen(false)} />
        </nav>
      </SheetContent>
    </Sheet>
  );
}

interface NavItemProps {
  href: string;
  icon: React.ReactNode;
  label: string;
  onClick?: () => void;
}

function NavItem({ href, icon, label, onClick }: NavItemProps) {
  return (
    <Link href={href}>
      <a 
        className="flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium hover:bg-accent transition-colors"
        onClick={onClick}
      >
        {icon}
        <span>{label}</span>
      </a>
    </Link>
  );
}
