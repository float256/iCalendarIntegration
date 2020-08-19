import { HttpClient } from "@angular/common/http";
import { Component } from '@angular/core';

import { Hotel } from 'src/app/shared/models/hotel.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public allHotels: Array<Hotel>;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.http.get('api/Hotel/GetAll').subscribe(
      (data: Array<Hotel>) => this.allHotels = data);
  }
}
