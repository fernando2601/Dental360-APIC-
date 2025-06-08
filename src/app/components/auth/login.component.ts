import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.formBuilder.group({
      username: ['admin', Validators.required],
      password: ['admin', Validators.required]
    });
  }

  ngOnInit() {
    // Auto-login for demo
    setTimeout(() => {
      this.onSubmit();
    }, 100);
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      
      // Simple demo authentication
      const credentials = this.loginForm.value;
      if (credentials.username === 'admin' && credentials.password === 'admin') {
        // Simulate successful login
        localStorage.setItem('token', 'demo-token');
        localStorage.setItem('user', JSON.stringify({
          id: 1,
          username: 'admin',
          fullName: 'Administrador',
          role: 'admin',
          email: 'admin@dentalspa.com'
        }));
        
        this.router.navigate(['/dashboard']);
      } else {
        this.errorMessage = 'Credenciais inválidas';
      }
      
      this.isLoading = false;
    }
  }
}