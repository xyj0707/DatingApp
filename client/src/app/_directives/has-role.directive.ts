import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective {
  @Input() appHasRole:string[]=[];
  user:User = {} as User;
  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
    private accountService: AccountService) {
       // Subscribe to the currentUser$ observable from AccountService to get the user information.
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user => {
          if (user) this.user = user;
        }
      })
    }

  ngOnInit(): void {
    // Check if the user has any roles specified in appHasRole.
    if (this.user?.roles.some(r => this.appHasRole.includes(r))) {
      // If the user has the required roles, create and display the embedded view.
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      // If the user does not have the required roles, clear the view container.
      this.viewContainerRef.clear();
    }
  }

}
