import { Injectable, Inject } from '@angular/core';
import * as signalR from "@aspnet/signalr";

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  public rougeApiretryAttempt = 0;
  private hubConnection: signalR.HubConnection

  constructor(@Inject('BASE_URL') private baseUrl: string) { }

  public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
                          .withUrl(this.baseUrl + 'undeterredApiHub')
                          .build();
    this.hubConnection
      .start()
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err))
  }

  public addRougeApiIssueListener = () => {
    this.hubConnection.on('rouge_api_issue', (data) => {
      this.rougeApiretryAttempt = data;
      console.log(data);
    });
  }

}
