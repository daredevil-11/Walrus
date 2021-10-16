import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  public undeterredResponse = false;
  public disableGet = false;
  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }


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
