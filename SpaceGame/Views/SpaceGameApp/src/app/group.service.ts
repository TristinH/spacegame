import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})

export class GroupService {
  private headers: HttpHeaders = new HttpHeaders({'Content-Type': 'application/json; charset=utf-8'});
  private serviceUrl: string = 'http://localhost:51758/groups';

  constructor(private http: HttpClient) { }

  public getGroups() {
    return this.http.get(this.serviceUrl, { headers: this.headers });
  }
}
