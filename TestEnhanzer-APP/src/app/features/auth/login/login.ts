import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

import { AuthService } from '../../../core/services/auth.service';
import { SpinnerComponent } from '../../../shared/components/spinner/spinner';
import { AlertComponent } from '../../../shared/components/alert/alert';

@Component({
  selector: 'app-login',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, SpinnerComponent, AlertComponent],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly loading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly showPassword = signal(false);

  protected readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(1)]],
  });

  protected get email() {
    return this.form.controls.email;
  }

  protected get password() {
    return this.form.controls.password;
  }

  protected togglePassword(): void {
    this.showPassword.update((v) => !v);
  }

  protected submit(): void {
    this.errorMessage.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const { email, password } = this.form.getRawValue();

    // #region agent log
    fetch('http://127.0.0.1:7281/ingest/2245c3a6-e918-4d40-a357-bbf19b48c63a',{method:'POST',headers:{'Content-Type':'application/json','X-Debug-Session-Id':'c14250'},body:JSON.stringify({sessionId:'c14250',runId:'pre-fix',hypothesisId:'C',location:'login.ts:submit',message:'submit clicked',data:{hasEmail:!!email,clientTime:new Date().toISOString()},timestamp:Date.now()})}).catch(()=>{});
    // #endregion

    this.auth
      .login({ email, password })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: () => this.router.navigate(['/purchase-bill']),
        error: (err: HttpErrorResponse) => {
          // #region agent log
          fetch('http://127.0.0.1:7281/ingest/2245c3a6-e918-4d40-a357-bbf19b48c63a',{method:'POST',headers:{'Content-Type':'application/json','X-Debug-Session-Id':'c14250'},body:JSON.stringify({sessionId:'c14250',runId:'pre-fix',hypothesisId:'A',location:'login.ts:error',message:'resolved login error for UI',data:{status:err.status,statusText:err.statusText,uiMessage:this.resolveError(err)},timestamp:Date.now()})}).catch(()=>{});
          // #endregion
          this.errorMessage.set(this.resolveError(err));
        },
      });
  }

  private resolveError(err: HttpErrorResponse): string {
    if (err.status === 0) {
      return 'Cannot reach the server. Please make sure the API is running.';
    }
    if (err.error?.message) {
      return err.error.message;
    }
    if (err.status === 401) {
      return 'Invalid email or password.';
    }
    return 'Something went wrong. Please try again.';
  }
}
