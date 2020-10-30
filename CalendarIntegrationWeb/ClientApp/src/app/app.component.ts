import {Component, OnInit} from '@angular/core';
import {HubConnection, HubConnectionBuilder} from '@aspnet/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit{
  title = 'app';
  private hubConnection: HubConnection;

  ngOnInit() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:57328/RoomUploadStatus')
      .build();
    this.hubConnection.start()
      .then(() => console.log('Connection started!'))
      .catch(err => console.log('Error while establishing connection :('));
    this.hubConnection.on('RoomUploadStatus', (data) => {
      console.log(data)
    });
    this.hubConnection.onclose(() => {
      setTimeout(function(){
        this.hubConnection.start()
          .then(() => console.log('Connection started!'))
          .catch(err => console.log('Error while establishing connection :('));
        this.hubConnection.on('RoomUploadStatus', (data) => {
          console.log(data)
        });
      },3000);
    });
  }
}
