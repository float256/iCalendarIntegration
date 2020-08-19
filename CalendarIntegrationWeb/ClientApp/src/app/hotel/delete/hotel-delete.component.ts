import { Component } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient, HttpHeaders} from "@angular/common/http";

import { Hotel } from "../../shared/models/hotel.model";

@Component({
  selector: 'app-hotel-delete',
  templateUrl: './hotel-delete.component.html',
})
export class HotelDeleteComponent {
  public hotel: Hotel;
  public hotelId: number;

  constructor(private route: ActivatedRoute, private router: Router, private http: HttpClient) {
    this.hotelId = this.route.snapshot.params.id;
  }

  ngOnInit() {
    this.http.get(`api/Hotel/${this.hotelId}`).subscribe(
      (data: Hotel) => this.hotel = data);
  }

  onSubmit(){
    let httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    };
    this.http.post('api/Hotel/Delete', this.hotelId, httpOptions).subscribe(
      (data: Hotel) => this.router.navigate(['/']));
  }
}
