import ShowAnnouncement from '../../views/announcements/ShowAnnouncement'
import CreateOrUpdateAnnouncement from '../../views/announcements/CreateOrUpdateAnnouncement'

export default {
  ANNOUNCEMENT: {
    path: '/announcement/:id',
    name: 'announcement',
    component: ShowAnnouncement
  },
  EDITANNOUNCEMENT: {
    path: '/announcement/:id/edit',
    name: 'EditAnnouncement',
    component: CreateOrUpdateAnnouncement
  },
  ADDANNOUNCEMENT: {
    path: '/announcement/add',
    name: 'AddAnnouncement',
    component: CreateOrUpdateAnnouncement
  }
}
