
import { Directive, Input, SimpleChanges, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Directive({
  selector: '[appRole],[appRoleOwner]',
})
export class RoleDirective {
  @Input('appRole') roles: string[] = [];
  @Input('appRoleOwner') isOwner: boolean = false;

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authService: AuthService
  ) {}

  ngOnChanges(changes: SimpleChanges) {
    if (this.authService.hasRole(this.roles) || this.isOwner) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }

}