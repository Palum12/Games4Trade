<template>
<div class="no-gutters">
    <div>
        <div id="login-form" class="row no-gutters p-2">
            <div class="form rounded col-12 p-3">
                <form @submit.prevent="onSubmit">
                    <div class="form-group">
                        <label for="title">Tytuł</label>
                        <input
                                type="text"
                                id="title"
                                class="form-control"
                                v-model="announcement.title">
                    </div>
                    <div class="form-group">
                        <label for="content">Treść</label>
                        <textarea
                                id="content"
                                class="form-control"
                                v-model="announcement.content">
                        </textarea>
                    </div>
                    <div v-if="isEditing" class="form-group d-flex justify-content-end">
                        <div class="submit mx-3">
                            <button type="submit" class="btn btn-info btn-block">Modyfikuj</button>
                        </div>
                        <div class="submit">
                            <button type="submit" class="btn btn-danger btn-block">Usuń</button>
                        </div>
                    </div>
                    <div v-if="!isEditing" class="form-group">
                        <div class="submit">
                            <button type="submit" class="btn btn-info btn-block">Dodaj</button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>
</template>

<script>
export default {
  name: 'CreateOrUpdateAnnouncement',
  data () {
    return {
      isEditing: false,
      announcement: {
        title: '',
        content: '',
        author: '',
        dateCreated: ''
      }
    }
  },
  beforeRouteEnter (to, from, next) {
    next(vm => {
      if (vm.$store.getters.isAdmin) {
        next()
      } else {
        next('/')
      }
    })
  },
  mounted () {
    if (this.$route.params.id) {
      let vm = this
      let id = this.$route.params.id
      this.$store.dispatch('getAnnouncement', id)
        .then(response => {
          vm.isEditing = true
          vm.announcement = response.data
          vm.announcement.dateCreated = vm.announcement.dateCreated.substring(0, 10) + ' ' +
            vm.announcement.dateCreated.substring(11, 16)
        })
        .catch(() => {
          vm.$router.push({name: 'home'})
        })
    }
  }
}
</script>

<style scoped>
.myText {
    width:100%;
    height:100%;
    box-sizing: border-box;         /* For IE and modern versions of Chrome */
    -moz-box-sizing: border-box;    /* For Firefox                          */
    -webkit-box-sizing: border-box; /* For Safari                           */
}
</style>
