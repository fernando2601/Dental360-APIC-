import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | Promise<boolean> | boolean {
    return this.authService.currentUser$.pipe(
      take(1),
      map(user => {
        if (user && this.authService.isAuthenticated()) {
          // Verificar se a rota requer roles específicas
          const requiredRoles = route.data?.['roles'] as string[];
          if (requiredRoles && requiredRoles.length > 0) {
            if (!this.authService.hasAnyRole(requiredRoles)) {
              this.router.navigate(['/access-denied']);
              return false;
            }
          }
          return true;
        }

        // Usuário não autenticado, redirecionar para login
        this.router.navigate(['/auth/login'], {
          queryParams: { returnUrl: state.url }
        });
        return false;
      })
    );
  }
}

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(): boolean {
    if (this.authService.isAuthenticated() && this.authService.isAdmin()) {
      return true;
    }

    this.router.navigate(['/access-denied']);
    return false;
  }
}

@Injectable({
  providedIn: 'root'
})
export class ManagerGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(): boolean {
    if (this.authService.isAuthenticated() && this.authService.isManager()) {
      return true;
    }

    this.router.navigate(['/access-denied']);
    return false;
  }
}