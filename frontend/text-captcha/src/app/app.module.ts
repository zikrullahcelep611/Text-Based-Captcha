import { NgModule } from '@angular/core';
import { BrowserModule, provideClientHydration, withEventReplay } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { QuestionComponent } from './question/question.component';
import { PresentationComponent } from './presentation/presentation.component';
import { RegisterComponent } from './register/register.component';
import { AuthInterceptor } from './auth.interceptor';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HomeComponent } from './home/home.component';
import { CustomAlertComponent } from './shared/custom-alert/custom-alert.component';
import { ReportComponent } from './report/report.component';
import { LayoutComponent } from './layout/layout.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import { CaptchaComponent } from './captcha/captcha.component';
import { CreateCaptchaComponent } from './create-captcha/create-captcha.component';


@NgModule({
  declarations: [
    AppComponent,
    QuestionComponent,
    PresentationComponent,
    RegisterComponent,
    DashboardComponent,
    HomeComponent,
    CustomAlertComponent,
    ReportComponent,
    LayoutComponent,
    SidebarComponent,
    CaptchaComponent,
    CreateCaptchaComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    HttpClientModule,
    LoginComponent,
    FormsModule
  ],
  providers: [
    provideClientHydration(withEventReplay()),
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
