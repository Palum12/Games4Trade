<template>
    <div class="row no-gutters">
        <div class="col-md-8 col-12 p-3">
            <div class="jumbotron jumbotron-fluid text-center ">
                <img alt="Vue logo" src="../assets/logo.png">
            </div>
        </div>
        <div class="col-md-4 col-12 p-3">
            <h3>Ogłoszenia dla społeczności</h3>
            <div >
                <announcements-list class="announcements"></announcements-list>
            </div>
        </div>
    </div>
</template>

<script>
import announcementsList from './announcements/AnnouncementsList'
export default {
  name: 'home',
  components: {
    announcementsList
  }
}
</script>

<style scoped>
.announcements {
    min-height: 200px;
    height: 78vh;
    max-height: 100%;
}
</style>
