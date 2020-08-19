import { Component } from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClient, HttpHeaders} from "@angular/common/http";

import {Room} from "../../shared/models/room.model";
import {Hotel} from "../../shared/models/hotel.model";

@Component({
  selector: 'app-hotel-delete',
  templateUrl: './room-delete.component.html',
})
export class RoomDeleteComponent {
  public roomId: number;
  public room: Room;

  constructor(private route: ActivatedRoute, private router: Router, private http: HttpClient) {
    this.roomId = this.route.snapshot.params.id;
  }

  public ngOnInit() {
    this.http.get(`api/Room/${this.roomId}`).subscribe(
      (data: Room) => this.room = data);
  }

  public onSubmit() {
    let httpOptions = {
      headers: new HttpHeaders({'Content-Type': 'application/json'})
    };
    this.http.post('api/Room/Delete', this.roomId, httpOptions).subscribe(
      (data: Hotel) => this.router.navigate([`/Hotel/${this.room.hotelId}`]));
  }
}
