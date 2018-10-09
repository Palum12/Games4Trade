<template>
    <div>
    <div class="form rounded m-2 p-3 genres">
        <form v-on:submit.prevent>
            <div v-if="genres!==null" v-for="genre in genres" :key="genre.id">
                <div class="form-row justify-content-between text-center mb-2">
                    <div class=" col-lg-7 col-md-6 col-12">
                        <input
                                type="text"
                                id="key"
                                readonly
                                class="form-control"
                                v-model="genre.value"
                        >
                    </div>
                    <button
                            v-if="isLiked(genre)"
                            class="btn btn-success ml-sm-1"
                            @click="dislike(genre)">Lubisz ten gatunek!</button>
                    <button
                            v-else
                            class="btn btn-primary ml-sm-1"
                            @click="like(genre)">Polub ten gatunek!</button>
                </div>
            </div>
        </form>
    </div>
        <button class="btn btn-primary btn-block" @click="saveLikedGenres">Zapisz</button>
    </div>
</template>

<script>
import mixins from '../../mixins/mixins'
import axios from 'axios'
export default {
  name: 'MyGenres',
  data () {
    return {
      genres: null,
      likedGenres: null
    }
  },
  props: {
    userId: Number
  },
  methods: {
    saveLikedGenres () {
      let vm = this
      mixins.methods.confirmationDialog(vm)
        .then(() => {
          let likedGenresRequest = []
          vm.likedGenres.forEach(el => {
            likedGenresRequest.push(el.id)
          })
          axios.patch(`users/${vm.userId}/genres`, likedGenresRequest)
            .then(response => {
              mixins.methods.simpleSuccessPopUp(vm)
              vm.$forceUpdate()
            })
            .catch(error => {
              mixins.methods.errorPopUp(error.response.data)
              vm.$forceUpdate()
            })
        })
        .catch()
    },
    getGenres () {
      let vm = this
      this.$store.dispatch('getGenres')
        .then(() => {
          vm.genres = vm.$store.getters.genres
        })
    },
    getUserGenres () {
      let vm = this
      return this.$store.dispatch('getLikedGenres', this.userId)
        .then(response => {
          vm.likedGenres = response.data
        })
    },
    isLiked (genre) {
      if (this.likedGenre !== null) {
        return this.likedGenres.some(x => x.id === genre.id)
      }
      return false
    },
    like (genre) {
      this.likedGenres.push(genre)
    },
    dislike (genre) {
      this.likedGenres = this.likedGenres.filter(el => el.id !== genre.id)
    }
  },
  async mounted () {
    await this.getUserGenres()
    this.getGenres()
  }
}
</script>

<style scoped>
    .genres {
        min-height: 200px;
        height: 62vh;
        max-height: 90%;
        overflow: hidden;
        overflow-y: auto;
    }
    input[readonly] {
        background-color: #fff;
    }
</style>
