import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/patination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit{
  members: Member[] | undefined;
  predicate ='liked';
  pageNumber = 1;
  pageSize = 5;
  pagination:Pagination | undefined;

  constructor(private memberService:MembersService){

  }
  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    this.memberService.getLikes(this.predicate,this.pageNumber,this.pageSize).subscribe({
      next:response =>{
        this.members= response.result;
        this.pagination = response.pagination;
      }
    })
  }

  pageChanged(event:any){
    // user interactions that may trigger the (pageChanged) event typically include:
    // Clicking on a specific page number.
    // Clicking on the "Previous" or "Next" buttons.
    // Clicking on the "First" or "Last" buttons.
        if(this.pageNumber !== event.page){
          this.pageNumber =   event.page;

          this.loadLikes();
        }

      }



}
