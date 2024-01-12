
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title : string = 'Dating App';


  constructor(private accountService:AccountService){ }

  ngOnInit(): void {


    this.setCurrentUser();
  }

  setCurrentUser(){
    const userStrng = localStorage.getItem('user');
    if(!userStrng) return;
    const user:User = JSON.parse(userStrng);
    this.accountService.setCurrentUser(user);
  }
}
