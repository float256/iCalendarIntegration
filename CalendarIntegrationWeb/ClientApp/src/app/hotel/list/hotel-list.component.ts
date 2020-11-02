import { HttpClient } from "@angular/common/http";
import { ActivatedRoute } from "@angular/router";
import { Component, OnInit } from '@angular/core';
import { Room } from "../../shared/models/room.model";
import { Hotel } from "../../shared/models/hotel.model";
import {RoomUploadStatus} from "../../shared/models/roomuploadstatus.model";
import {HubConnection, HubConnectionBuilder} from "@aspnet/signalr";

@Component({
  selector: 'app-hotel-list',
  templateUrl: './hotel-list.component.html',
})
export class HotelListComponent {
  public hotel: Hotel;
  public hotelId: number;
  public allHotelRooms: Array<Room>;
  public allHotelRoomsStatuses: Map<number, RoomUploadStatus> = new Map<number, RoomUploadStatus>();

  private hubConnection: HubConnection;

  constructor(private http: HttpClient, private route: ActivatedRoute) { }

  private setupSignalRConnection(hubConnection: HubConnection, allHotelRoomsStatuses: Map<number, RoomUploadStatus>) {
    hubConnection.start()
      .then(() => console.log('Connection started!'))
      .catch((err: Error) => console.log(`Error while establishing connection: ${err.message}`));
    hubConnection.on('transferRoomUploadStatus', (roomUploadStatuses: Array<RoomUploadStatus>) => {
      for (let i = 0; i < roomUploadStatuses.length; i++) {
        allHotelRoomsStatuses.set(roomUploadStatuses[i].roomId, roomUploadStatuses[i]);
      }
    });
  }

  ngOnInit() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('./RoomUploadStatusHub')
      .build();
    this.setupSignalRConnection(this.hubConnection, this.allHotelRoomsStatuses);
    this.hubConnection.onclose(() => {
      setTimeout(() => this.setupSignalRConnection(this.hubConnection, this.allHotelRoomsStatuses),3000);
    });

    this.hotelId = this.route.snapshot.params.id;
    this.http.get(`/api/Hotel/${this.hotelId}`).subscribe(
      (data: Hotel) => this.hotel = data
    );
    this.http.get(`/api/Room/GetByHotelId/${this.hotelId}`).subscribe(
      (data: Array<Room>) => this.allHotelRooms = data
    );
  }

  isSuccessStatus(roomId): boolean {
    return (this.allHotelRoomsStatuses.get(roomId).status.toLowerCase() == 'ok');
  }

  getRoomStatus(roomId: number): string {
    if(this.allHotelRoomsStatuses.has(roomId)) {
      return (this.allHotelRoomsStatuses.get(roomId).status);
    } else {
      return "";
    }
  }

  getRoomStatusMessage(roomId: number): string {
    if(this.allHotelRoomsStatuses.has(roomId)) {
      return (this.allHotelRoomsStatuses.get(roomId).message);
    } else {
      return "";
    }
  }
}
