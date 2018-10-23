import ShowAnnouncement from '../views/announcements/ShowAnnouncement'
import CreateOrUpdateAnnouncement from '../views/announcements/CreateOrUpdateAnnouncement'

export default {
  ADDANNOUNCEMENT: {
    path: '/announcements/add',
    name: 'AddAnnouncement',
    component: CreateOrUpdateAnnouncement
  },
  EDITANNOUNCEMENT: {
    path: '/announcements/:id/edit',
    name: 'EditAnnouncement',
    component: CreateOrUpdateAnnouncement
  },
  ANNOUNCEMENT: {
    path: '/announcements/:id',
    name: 'announcement',
    component: ShowAnnouncement
  }
}
