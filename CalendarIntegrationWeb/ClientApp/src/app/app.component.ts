import {Component, OnInit} from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit{
  title = 'app';
  private hubConnection: HubConnection;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('./RoomUploadStatusHub')
      .build();
    this.hubConnection.start()
      .then(() => console.log('Connection started!'))
      .catch(err => console.log('Error while establishing connection :('));
    this.hubConnection.on('transferRoomUploadStatus', (data) => {
      console.log(data)
    });
    this.hubConnection.onclose(() => {
      setTimeout(function(){
        this.hubConnection.start()
          .then(() => console.log('Connection started!'))
          .catch(err => console.log('Error while establishing connection :('));
        this.hubConnection.on('transferRoomUploadStatus', (data) => {
          console.log(data)
        });
      },3000);
    });

    //this.startHttpRequest();
  }

  private startHttpRequest = () => {
    this.http.get('./api/RoomUploadStatus')
      .subscribe(res => {
        console.log(res);
      })
  }
}
