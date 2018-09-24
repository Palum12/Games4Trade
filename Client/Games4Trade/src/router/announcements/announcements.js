import ShowAnnouncement from '../../views/announcements/ShowAnnouncement'
import CreateOrUpdateAnnouncement from '../../views/announcements/CreateOrUpdateAnnouncement'

export default {
  ADDANNOUNCEMENT: {
    path: '/announcement/add',
    name: 'AddAnnouncement',
    component: CreateOrUpdateAnnouncement
  },
  EDITANNOUNCEMENT: {
    path: '/announcement/:id/edit',
    name: 'EditAnnouncement',
    component: CreateOrUpdateAnnouncement
  },
  ANNOUNCEMENT: {
    path: '/announcement/:id',
    name: 'announcement',
    component: ShowAnnouncement
  }
}
