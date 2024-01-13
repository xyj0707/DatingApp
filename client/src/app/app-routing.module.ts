import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MembersDetailComponent } from './members/members-detail/members-detail.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { authGuard } from './_guards/auth.guard';

const routes: Routes = [
  {path:'',component:HomeComponent},
  {path:'',
    runGuardsAndResolvers:'always',
    canActivate:[authGuard],
    children:[
      {path:'members',component:MemberListComponent},
      {path:'members/:id',component:MembersDetailComponent},
      {path:'lists',component:ListsComponent},
      {path:'messages',component:MessagesComponent},

    ]},

  {path:'**',component:HomeComponent,pathMatch:'full'},


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
