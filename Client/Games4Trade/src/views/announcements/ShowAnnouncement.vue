<template>
    <div class="no-gutters announcement">
        <div class="container-fluid  mb-3">
            <h1 class="font-weight-bold mb-4">{{announcement.title}}</h1>
            <small>Data utworzenia: {{announcement.dateCreated}}</small>
            <br>
            <small>Autor: {{announcement.author}}</small>
        </div>
        <div class="container-fluid" style="white-space: pre-line;">
            {{announcement.content}}
        </div>
        <button class="btn btn-info ml-3" @click="goBack">Powrót</button>
    </div>
</template>

<script>
export default {
  name: 'ShowAnnouncement',
  data () {
    return {
      announcement: {
        title: String,
        content: String,
        author: String,
        dateCreated: String
      }
    }
  },
  methods: {
    goBack () {
      this.$router.go(-1)
    }
  },
  mounted () {
    let vm = this
    let id = this.$route.params.id
    this.$store.dispatch('getAnnouncement', id)
      .then(response => {
        vm.announcement = response.data
        vm.announcement.dateCreated = vm.announcement.dateCreated.substring(0, 10) + ' ' +
          vm.announcement.dateCreated.substring(11, 16)
      })
      .catch(() => { this.$router.push({name: 'home'}) })
  }
}
</script>

<style scoped>
.announcement {
    margin: calc(10px + 2vh) 2vw;
    width: 90vw;
    height: 90vh;
    text-justify: newspaper;
}
</style>
