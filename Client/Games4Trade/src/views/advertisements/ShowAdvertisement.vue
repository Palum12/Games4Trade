<template>
<div v-if="hasDataLoaded" class="no-gutters advertisement">
    <div class="row">
        <div v-if="activePhotoId != null" class="col-7">
            <div id="carousel ExampleControls" class="carousel slide" data-ride="carousel">
                <div class="carousel-inner myTest">
                    <div class="carousel-item active">
                        <img class="d-block w-100"
                             :src="`http://localhost:5000/api/advertisements/${advertisement.id}/photos/${currentPhotoId()}`">
                    </div>
                </div>
                <a v-if="hasPreviousPhoto" class="carousel-control-prev" role="button" data-slide="prev" @click="changePhoto(-1)">
                    <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                    <span class="sr-only">Previous</span>
                </a>
                <a v-if="hasNextPhoto" class="carousel-control-next" role="button" data-slide="next" @click="changePhoto(1)">
                    <span class="carousel-control-next-icon" aria-hidden="true"></span>
                    <span class="sr-only">Next</span>
                </a>
            </div>
        </div>
    </div>
</div>
</template>

<script>
import axios from 'axios'
export default {
  name: 'ShowAdvertisement',
  data () {
    return {
      userId: null,
      hasDataLoaded: false,
      isOwner: false,
      advertisement: null,
      activePhotoId: null,
      hasNextPhoto: false,
      hasPreviousPhoto: false
    }
  },
  methods: {
    changePhoto (val) {
      if (val > 0) {
        this.hasPreviousPhoto = true
        this.activePhotoId = this.activePhotoId + 1
        if (this.activePhotoId === this.advertisement.photos.length - 1) {
          this.hasNextPhoto = false
        }
      } else {
        this.hasNextPhoto = true
        this.activePhotoId = this.activePhotoId - 1
        if (this.activePhotoId === 0) {
          this.hasPreviousPhoto = false
        }
      }
    },
    currentPhotoId () {
      return this.advertisement.photos[this.activePhotoId].id
    }
  },
  async mounted () {
    let vm = this
    await this.$store.dispatch('getUserId')
      .then(response => {
        vm.userId = response.data
      })
    let id = this.$route.params.id
    await axios.get(`advertisements/${id}`)
      .then(response => {
        vm.advertisement = response.data
        vm.isOwner = vm.userId === response.data.userId
        vm.hasDataLoaded = true
        if (vm.advertisement.photos.length > 0) {
          vm.activePhotoId = 0
        }
        if (vm.advertisement.photos.length > 1) {
          vm.hasNextPhoto = true
        }
      })
      .catch(() => {
        vm.$router.push('/')
      })
  }
}
</script>

<style scoped>
    .advertisement {
        margin: 0 2vw;
        padding-bottom: 2vh;
        width: 90vw;
        text-justify: newspaper;
    }
    img {
        height:300px;
        object-fit: cover;
        border: 1px solid lightgray;
        border-radius: 5px;
    }
    .myTest {
        height:300px;
        border: 1px solid lightgray;
        border-radius: 5px;
    }
</style>
