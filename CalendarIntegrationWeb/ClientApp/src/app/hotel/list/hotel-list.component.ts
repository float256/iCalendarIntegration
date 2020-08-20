import {HttpClient} from "@angular/common/http";
import { ActivatedRoute } from "@angular/router";
import { Component } from '@angular/core';

import { Room } from "../../shared/models/room.model";
import { Hotel } from "../../shared/models/hotel.model";

@Component({
  selector: 'app-hotel-list',
  templateUrl: './hotel-list.component.html',
})
export class HotelListComponent {
  public hotel: Hotel;
  public hotelId: number;
  public allHotelRooms: Array<Room>;

  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  ngOnInit() {
    this.hotelId = this.route.snapshot.params.id;
    this.http.get(`/api/Hotel/${this.hotelId}`).subscribe(
      (data: Hotel) => this.hotel = data
    );
    this.http.get(`/api/Room/GetByHotelId/${this.hotelId}`).subscribe(
      (data: Array<Room>) => this.allHotelRooms = data
    );
  }
}
