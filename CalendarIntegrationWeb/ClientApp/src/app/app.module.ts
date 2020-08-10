import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';

import { HotelListComponent } from './hotel/list/hotel-list.component';
import { HotelEditComponent } from './hotel/edit/hotel-edit.component';
import { HotelAddComponent } from './hotel/add/hotel-add.component';
import { HotelDeleteComponent } from './hotel/delete/hotel-delete.component';

import { RoomAddComponent } from './room/add/room-add.component';
import { RoomDeleteComponent } from './room/delete/room-delete.component';
import { RoomEditComponent } from './room/edit/room-edit.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    HotelListComponent,
    HotelEditComponent,
    HotelAddComponent,
    HotelDeleteComponent,
    RoomAddComponent,
    RoomDeleteComponent,
    RoomEditComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      {
        path: 'Hotel',
        children: [
          {
            path: 'Add',
            component: HotelAddComponent
          },
          {
            path: 'Delete/:id',
            component: HotelDeleteComponent
          },
          {
            path: 'Edit/:id',
            component: HotelEditComponent
          },
          {
            path: ':id',
            component: HotelListComponent
          }
        ]
      },
      {
        path: 'Room',
        children: [
          {
            path: 'Add/:id',
            component: RoomAddComponent
          },
          {
            path: 'Delete/:id',
            component: RoomDeleteComponent
          },
          {
            path: 'Edit/:id',
            component: RoomEditComponent
          }
        ]
      }
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
