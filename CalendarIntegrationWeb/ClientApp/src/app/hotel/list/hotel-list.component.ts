import { HttpClient } from "@angular/common/http";
import {ActivatedRoute, NavigationEnd, NavigationStart, Router} from "@angular/router";
import {Component, HostListener, OnInit} from '@angular/core';
import { Room } from "../../shared/models/room.model";
import { Hotel } from "../../shared/models/hotel.model";
import {RoomUploadStatus} from "../../shared/models/roomuploadstatus.model";
import {HubConnection, HubConnectionBuilder} from "@aspnet/signalr";
import {Subscription} from "rxjs";

export let browserRefresh: boolean = false;

@Component({
  selector: 'app-hotel-list',
  templateUrl: './hotel-list.component.html',
})
export class HotelListComponent {
  public hotel: Hotel;
  public hotelId: number;
  public allHotelRooms: Array<Room> = new Array<Room>();

  private hubConnection: HubConnection;
  private isOpenedConnection: boolean;

  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router) {
    this.isOpenedConnection = true;
    this.router.events.subscribe(async (ev) => {
      if ((ev instanceof NavigationEnd) && (this.hubConnection !== undefined)) {
        this.isOpenedConnection = false;
        await this.hubConnection.stop();
        this.hubConnection = undefined;
        console.log('Connection closed!');
      }
    });
  }


  private setupSignalRConnection() {
    this.hubConnection.start()
      .then(() => console.log('Connection started!'))
      .catch((err: Error) => console.log(`Error while establishing connection: ${err.message}`));
    this.hubConnection.on('transferRoomUploadStatus', (roomUploadStatus: RoomUploadStatus) => {
      let currRoom = this.allHotelRooms.filter((room) => room.id == roomUploadStatus.roomId)[0];
      let currRoomArrayIdx = this.allHotelRooms.indexOf(currRoom);
      this.allHotelRooms[currRoomArrayIdx].status = roomUploadStatus.status;
      this.allHotelRooms[currRoomArrayIdx].statusMessage = roomUploadStatus.message;
    });
  }

  ngOnInit() {
    this.hotelId = this.route.snapshot.params.id;
    this.http.get(`/api/Hotel/${this.hotelId}`).subscribe(
      (data: Hotel) => this.hotel = data
    );
    this.http.get(`/api/Room/GetByHotelId/${this.hotelId}`).subscribe(
      (data: Array<Room>) => this.allHotelRooms = data
    );

    this.hubConnection = new HubConnectionBuilder()
      .withUrl('./RoomUploadStatusHub')
      .build();
    this.setupSignalRConnection();
    this.hubConnection.onclose(() => {
      if (this.isOpenedConnection) {
        setTimeout(() => this.setupSignalRConnection(), 3000);
      }
    });
  }

  isSuccessStatus(roomStatus: string): boolean {
    return (roomStatus.toLowerCase() == 'ok');
  }

}
