import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SignalRService } from '../services/signal-r.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {

  public undeterredResponse = false;
  public disableGet = false;
  
  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    public signalRService: SignalRService) { }

  ngOnInit() {
    this.signalRService.startConnection();
    this.signalRService.addRougeApiIssueListener();
  }

  async OnGetClick() {

    // reset the response ...
    this.undeterredResponse = false;
    this.disableGet = true;

    // try fetching again ...
    try {
      var response = await this.http.get<boolean>(this.baseUrl + 'Undeterred/Get').toPromise();
      this.undeterredResponse = response;
    } catch (error) {
      console.error(error)
    } finally {
      this.disableGet = false;
    }

  }
}
