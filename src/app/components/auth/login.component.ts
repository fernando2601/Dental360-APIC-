import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  template: `
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <div class="logo">
            <span class="logo-icon">DS</span>
            <span class="logo-text">DentalSpa</span>
          </div>
          <h2>Acesso ao Sistema</h2>
        </div>
        
        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label>Email</label>
            <input type="email" formControlName="email" class="form-control" placeholder="seu@email.com">
            <div class="error" *ngIf="loginForm.get('email')?.invalid && loginForm.get('email')?.touched">
              Email é obrigatório
            </div>
          </div>
          
          <div class="form-group">
            <label>Senha</label>
            <input type="password" formControlName="password" class="form-control" placeholder="********">
            <div class="error" *ngIf="loginForm.get('password')?.invalid && loginForm.get('password')?.touched">
              Senha é obrigatória
            </div>
          </div>
          
          <div class="error" *ngIf="loginError">{{ loginError }}</div>
          
          <button type="submit" class="btn btn-primary btn-full" [disabled]="!loginForm.valid || loading">
            {{ loading ? 'Entrando...' : 'Entrar' }}
          </button>
        </form>
        
        <div class="demo-credentials">
          <p><strong>Credenciais de demonstração:</strong></p>
          <p>Email: admin@dentalspa.com</p>
          <p>Senha: admin123</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      padding: 2rem;
    }
    
    .login-card {
      background: white;
      border-radius: 12px;
      padding: 3rem;
      box-shadow: 0 10px 25px rgba(0,0,0,0.1);
      width: 100%;
      max-width: 400px;
    }
    
    .login-header {
      text-align: center;
      margin-bottom: 2rem;
    }
    
    .logo {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 0.75rem;
      margin-bottom: 1rem;
    }
    
    .logo-icon {
      width: 3rem;
      height: 3rem;
      background: #3b82f6;
      border-radius: 0.75rem;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 700;
      font-size: 1.25rem;
      color: white;
    }
    
    .logo-text {
      font-size: 1.5rem;
      font-weight: 700;
      color: #1f2937;
    }
    
    .login-header h2 {
      margin: 0;
      color: #1f2937;
      font-weight: 600;
    }
    
    .form-group {
      margin-bottom: 1.5rem;
    }
    
    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #374151;
    }
    
    .form-control {
      width: 100%;
      padding: 0.875rem;
      border: 1px solid #d1d5db;
      border-radius: 6px;
      font-size: 1rem;
      transition: border-color 0.2s;
    }
    
    .form-control:focus {
      outline: none;
      border-color: #3b82f6;
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
    }
    
    .btn {
      padding: 0.875rem 1.5rem;
      border: none;
      border-radius: 6px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
      font-size: 1rem;
    }
    
    .btn-primary {
      background: #3b82f6;
      color: white;
    }
    
    .btn-primary:hover:not(:disabled) {
      background: #2563eb;
    }
    
    .btn-full {
      width: 100%;
    }
    
    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
    
    .error {
      color: #ef4444;
      font-size: 0.875rem;
      margin-top: 0.5rem;
    }
    
    .demo-credentials {
      margin-top: 2rem;
      padding: 1rem;
      background: #f9fafb;
      border-radius: 6px;
      font-size: 0.875rem;
      color: #6b7280;
    }
    
    .demo-credentials p {
      margin: 0.25rem 0;
    }
  `]
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  loginError = '';
  returnUrl = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loginForm = this.fb.group({
      email: ['admin@dentalspa.com', [Validators.required, Validators.email]],
      password: ['admin123', Validators.required]
    });
  }

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
    
    // Se já estiver autenticado, redireciona
    if (this.authService.isAuthenticated()) {
      this.router.navigate([this.returnUrl]);
    }
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.loading = true;
      this.loginError = '';
      
      const { email, password } = this.loginForm.value;
      
      this.authService.login(email, password).subscribe({
        next: () => {
          this.router.navigate([this.returnUrl]);
        },
        error: (error) => {
          this.loginError = 'Email ou senha inválidos';
          this.loading = false;
        }
      });
    }
  }
}