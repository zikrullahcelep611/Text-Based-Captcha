import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QuestionComponent } from './question/question.component';
import { PresentationComponent } from './presentation/presentation.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './auth.guard';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HomeComponent } from './home/home.component';
import { ReportComponent } from './report/report.component';
import { LayoutComponent } from './layout/layout.component';

const routes: Routes = [
  { path: '', component: HomeComponent }, 
  {
    path: '',
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
      { path: 'question', component: QuestionComponent, canActivate: [AuthGuard] },
      { path: 'presentation', component: PresentationComponent },
      { path: 'report', component: ReportComponent },
      { path: 'register', component: RegisterComponent }
    ]
  },
  { path: 'login', component: LoginComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
