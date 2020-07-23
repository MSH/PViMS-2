import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { UserRoleModel } from '../models/user/user.role.model';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { NotificationModel } from '../models/user/notification.model';

@Injectable({ providedIn: 'root' })
export class AccountService extends BaseService {

    connected = true;
    userRoles: UserRoleModel[] = [];

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
        this.apiController = "/accounts";

        this.userRoles = this.getUserRoles();
    }

    login(model: any) {
        return this.Post(`login`, model);
    }

    register(model: any) {
        return this.Post(`Register`, model);
    }

    forgotPassword(model: any) {
        return this.Post(`ForgotPassword`, model);
    }

    resetPassword(model: any) {
        return this.Post(`ResetPassword`, model);
    }

    getUserRoles(): any {
      let parameters: ParameterKeyValueModel[] = [];
  
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '999'});
  
      let id = this.getUniquename();

      return this.Get<UserRoleModel[]>(`/users/${id}/roles`, 'application/vnd.pvims.identifier.v1+json', parameters);
    }

    getNotifications(): any {
      let parameters: ParameterKeyValueModel[] = [];

      return this.Get<NotificationModel[]>(`/accounts/notifications`, 'application/vnd.pvims.identifier.v1+json', parameters);
    }

    setConnectionStatus(connected: boolean): void {
      this.connected = connected;
    }

    isConnected(): boolean {
      return this.connected;
    }
}
