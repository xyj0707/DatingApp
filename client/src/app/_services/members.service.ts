import { HttpClient,  HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/patination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members:Member[]=[];
  memberCache = new Map();
  user:User | undefined;
  userParams: UserParams | undefined;

  constructor(private http : HttpClient, private accountService:AccountService) {
    // Subscribe to the currentUser$ observable to get the current user details.
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user =>{
        if(user){
          this.userParams = new UserParams(user);
          this.user = user;
        }
      }
    })
   }

  getUserParams(){
    return this.userParams;
  }

  setUserParams(params: UserParams){
    this.userParams = params;
  }

  resetUserParams(){
    if(this.user){
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  getMembers(userParams:UserParams){
    // Check if the response is already in the cache
    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response) return of(response);

    // If response is not in the cache, proceed to make the API request
    // Get pagination headers
    let params = this.getPaginationHeaders(userParams.pageNumber,userParams.pageSize);

    params = params.append('minAge',userParams.minAge);
    params = params.append('maxAge',userParams.maxAge);
    params = params.append('gender',userParams.gender);
    params = params.append('orderBy',userParams.orderBy);

    return this.getPaginatedResults<Member[]>(this.baseUrl + 'users',params).pipe(
      map(response => {
        this.memberCache.set(Object.values(userParams).join('-'),response);
        return response;
      })
    );
    // if(this.members.length > 0) return of(this.members);

    // return this.http.get<Member[]>(this.baseUrl+'users').pipe(
    //   map(members => {
    //     this.members = members;
    //     return members;
    //   })
    // )
  }
  getMember(username:string){
    // Retrieve a member from the memberCache based on username
    const member = [...this.memberCache.values()]// Convert the memberCache values to an array
      .reduce((arr,elem) => arr.concat(elem.result),[])// Concatenate the 'result' arrays from each cache entry into a single array
      .find((member:Member) => member.userName===username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl+'users/'+username)
  }

  updateMember(member:Member){
    return this.http.put(this.baseUrl + 'users',member).pipe(
      map(()=>{
        const index = this.members.indexOf(member);
        this.members[index] = {...this.members[index],...member}
      })
    );
  }


  setMainPhoto(photoId:number){
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId:number){
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  private getPaginatedResults<T>(url:string,params: HttpParams) {
    const paginatedResult:PaginatedResult<T> = new PaginatedResult<T>;
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map(response => {
        if (response.body) {
          paginatedResult.result = response.body;
        }
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          paginatedResult.pagination = JSON.parse(pagination);
        }

        return paginatedResult;

      }));
  }

  private getPaginationHeaders(pageNumber:number,pageSize:number) {
    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);

    return params;
  }
}
