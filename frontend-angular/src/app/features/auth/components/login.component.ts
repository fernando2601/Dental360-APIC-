import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  error = '';
  returnUrl = '';
  showPassword = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.loginForm = this.formBuilder.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  ngOnInit(): void {
    // Verificar se já está autenticado
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
      return;
    }

    // Obter URL de retorno
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/dashboard';
  }

  get f() { 
    return this.loginForm.controls; 
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.loading = true;
    this.error = '';

    const loginData = {
      username: this.f['username'].value.trim(),
      password: this.f['password'].value,
      rememberMe: this.f['rememberMe'].value
    };

    this.authService.login(loginData).subscribe({
      next: (response) => {
        this.loading = false;
        this.router.navigate([this.returnUrl]);
      },
      error: (error) => {
        this.loading = false;
        this.error = error.message || 'Erro ao fazer login. Verifique suas credenciais.';
      }
    });
  }

  toggleShowPassword(): void {
    this.showPassword = !this.showPassword;
  }

  navigateToRegister(): void {
    this.router.navigate(['/auth/register']);
  }

  navigateToForgotPassword(): void {
    this.router.navigate(['/auth/forgot-password']);
  }

  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  hasError(controlName: string, errorType: string): boolean {
    const control = this.loginForm.get(controlName);
    return !!(control?.hasError(errorType) && control?.touched);
  }

  getErrorMessage(controlName: string): string {
    const control = this.loginForm.get(controlName);
    
    if (control?.hasError('required')) {
      return `${this.getFieldName(controlName)} é obrigatório`;
    }
    
    if (control?.hasError('minlength')) {
      const minLength = control.getError('minlength').requiredLength;
      return `${this.getFieldName(controlName)} deve ter pelo menos ${minLength} caracteres`;
    }
    
    return '';
  }

  private getFieldName(controlName: string): string {
    const fieldNames: { [key: string]: string } = {
      username: 'Nome de usuário',
      password: 'Senha'
    };
    return fieldNames[controlName] || controlName;
  }
}