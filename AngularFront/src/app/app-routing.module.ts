import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { UserdateComponent } from './components/userdate/userdate.component';
import { AuthGuard } from './helpers/auth.guard';

const routes: Routes = [ {
  path: '',
  component: UserdateComponent,
  canActivate: [AuthGuard]
},
/* {
  path: 'admin',
  component: AdminComponent,
  canActivate: [AuthGuard],
  data: { roles: [Role.Admin] }
}, */
{
  path: 'login',
  component: LoginComponent
},

// otherwise redirect to home
{ path: '**', redirectTo: '' }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
