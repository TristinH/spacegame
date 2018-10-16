import { Component, OnInit, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

import { GroupService } from './group.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit, OnDestroy {
  private hubConnection: HubConnection;

  public groups: any[];
  public error: any;
  public intervals: any[];
  public currentGroup: any;

  constructor(private groupService: GroupService) {
    this.currentGroup = {};
    this.intervals = [];

    groupService.getGroups().subscribe(
      (data: any) => {
        this.groups = data.groups;
        for (let group of this.groups) {
          if (!group.startTime) {
            continue;
          }

          let startTime = new Date(group.startTime);
          group.elapsedSeconds = this.getTimeForView(startTime);
          let intervalId = setInterval(() => {
            group.elapsedSeconds = this.getTimeForView(startTime);
          }, 1000);

          this.intervals.push(intervalId);
        }
      },
      (error: any) => this.error = error.error.isTrusted);
  }

  private getTimeForView(startTime: Date) {
    let millis = new Date().getTime() - startTime.getTime();
    return Math.floor(millis / 1000);
  }

  ngOnInit() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:51758/notify')
      .build();

    this.hubConnection.start().catch(e => console.error(e));

    this.hubConnection.on('UpdateGroupClients', (groupData: any) => {
      console.log("here is the group data", groupData);
      this.currentGroup = groupData;
    });
  }

  ngOnDestroy() {
    for (let interval of this.intervals) {
      clearInterval(interval);
    }
  }
}
