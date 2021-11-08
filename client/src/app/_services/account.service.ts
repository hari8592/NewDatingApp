import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';
import { ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl=environment.apiUrl;
  //rpley subject is buffer for storage (1) means it can store one value
  private currentUserSource=new ReplaySubject<User>(1);
  currentUser$=this.currentUserSource.asObservable();

  constructor(private http:HttpClient) { }

  login(model:any){
    return this.http.post<User>(this.baseUrl+ 'account/login',model).pipe(
      map( ( response:User )  =>{
        const user=response;
        if( user ){
          localStorage.setItem('user',JSON.stringify(user));
          //buffer for storing user object-next() to access next content memory
          this.currentUserSource.next(user);
        }
      })
    )
  }

  register(model:any){
    return this.http.post<User>(this.baseUrl+ 'account/register',model).pipe(
      map((user:User)=>{
        if( user ){
          localStorage.setItem('user',JSON.stringify(user));
          this.currentUserSource.next(user);
        }
        //return user;
      })
    )
  }

  setCurrentUser(user:User){
    this.currentUserSource.next(user);
  }
  

  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
