import {HttpClient} from "@angular/common/http";
import { ActivatedRoute } from "@angular/router";
import { Component, OnInit } from '@angular/core';
import { Room } from "../../shared/models/room.model";
import { Hotel } from "../../shared/models/hotel.model";
import {RoomUploadStatus} from "../../shared/models/roomuploadstatus.model";

@Component({
  selector: 'app-hotel-list',
  templateUrl: './hotel-list.component.html',
})
export class HotelListComponent {
  public hotel: Hotel;
  public hotelId: number;
  public allHotelRooms: Array<Room>;
  public roomStatuses: Array<RoomUploadStatus> = [];

  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  ngOnInit() {
    this.hotelId = this.route.snapshot.params.id;
    this.http.get(`/api/Hotel/${this.hotelId}`).subscribe(
      (data: Hotel) => this.hotel = data
    );
    this.http.get(`/api/Room/GetByHotelId/${this.hotelId}`).subscribe(
      (data: Array<Room>) => {
        this.allHotelRooms = data;
        for (let i = 0; i < this.allHotelRooms.length; i++){
          let currRoom = this.allHotelRooms[i];
          this.http.get(`/api/RoomUploadStatus/GetByRoomId/${currRoom.id}`).subscribe(
            (data: RoomUploadStatus) => this.roomStatuses.push(data)
          );
        }
      }
    );
  }

  public getRoomStatus(roomId: number): string {
    let roomStatusesForRoom = this.roomStatuses.filter(data => data.roomId == roomId);
    if (roomStatusesForRoom.length == 0) {
      return "";
    } else {
      return roomStatusesForRoom[0].status;
    }
  }

  public getRoomStatusMessage(roomId: number): string{
    let roomStatusesForRoom = this.roomStatuses.filter(data => data.roomId == roomId);
    if (roomStatusesForRoom.length == 0) {
      return "";
    } else {
      return roomStatusesForRoom[0].message;
    }
  }
}
